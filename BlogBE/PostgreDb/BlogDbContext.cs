using BlogBE.Constants;
using Microsoft.EntityFrameworkCore;

//Entity framework is a bridge between C# code and db
//it translates C# code to SQL queries and vice versa

namespace BlogBE.PostgreDb;

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }

    public DbSet<Comment?> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //The reason why I have to specify is required here is that other field is value type which is non-nullable, so ef core will automatically make it required (i.e. not null)
        //For all reference types, remember to set it to required if you want it to be non-nullable
        modelBuilder.Entity<BlogPost>()
            .Property(post => post.Title)
            .HasMaxLength(BlogPostConstraint.MaxTitleLength)
            .IsRequired();

        modelBuilder.Entity<BlogPost>()
            .Property(post => post.Content)
            .IsRequired();

        modelBuilder.Entity<Comment>()
            .Property(comment => comment.Content)
            .IsRequired();
    }
}