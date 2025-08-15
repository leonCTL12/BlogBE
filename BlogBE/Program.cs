using System.Text;
using BlogBE.Data;
using BlogBE.DTO;
using BlogBE.Jwt;
using BlogBE.Middleware;
using BlogBE.MongoDb;
using BlogBE.User;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Add services to the container.
builder.Host.UseSerilog();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterUserValidator>();
builder.Services.AddScoped<UserService>();
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var mongoConn = builder.Configuration.GetConnectionString("Mongo")!;
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConn));
builder.Services.AddSingleton(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    var dbName = builder.Configuration["Mongo:Database"];
    return client.GetDatabase(dbName);
});

builder.Services.AddSingleton(serviceProvider =>
{
    var db = serviceProvider.GetRequiredService<IMongoDatabase>();
    var collectionName = builder.Configuration["Mongo:Collection"];
    return db.GetCollection<ActivityLog>(collectionName);
});
builder.Services.AddSingleton<ActivityLogService>();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
if (jwtOptions == null) throw new InvalidOperationException("JWT options are not configured properly.");
var key = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);

builder.Services.AddSingleton<JwtTokenFactory>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//This is the sequence of middleware that will be executed for each request
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRequestContext();
app.MapControllers();

app.Run();