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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BlogPost>()
            .Property(post => post.Title)
            .HasMaxLength(BlogPostConstraint.MaxTitleLength)
            .IsRequired();

        modelBuilder.Entity<BlogPost>()
            .Property(post => post.Content)
            .IsRequired();
    }
}