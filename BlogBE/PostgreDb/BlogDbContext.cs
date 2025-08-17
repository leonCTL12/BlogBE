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
}