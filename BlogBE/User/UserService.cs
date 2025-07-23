using BlogBE.Data;

namespace BlogBE.User;

public class UserService
{
    private readonly BlogDbContext _context;

    public UserService(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<User> RegisterAsync(User user)
    {
        user.Password= BCrypt.Net.BCrypt.HashPassword(user.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    
    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }
}