using BlogBE.Data;
using BlogBE.DTO;
using BlogBE.Middleware;
using BlogBE.MongoDb;
using BlogBE.User;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserValidator>();
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

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

//This is the sequence of middleware that will be executed for each request
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRequestContext();

app.MapControllers();

app.Run();