using Microsoft.EntityFrameworkCore;
//Entity framework is a bridge between C# code and db
//it translates C# code to SQL queries and vice versa

namespace BlogBE.Data;

public class BlogDbContext : DbContext
{
    public DbSet<User.User> Users { get; set; }

    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }
}