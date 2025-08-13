using BlogBE.Data;
using BlogBE.DTO;

namespace BlogBE.User;

public class UserService
{
    private readonly BlogDbContext _context;

    public UserService(BlogDbContext context)
    {
        _context = context;
    }

    public async Task<int> RegisterAsync(RegisterRequest dto)
    {
        var user = new DB.User
        {
            Email = dto.Email,
            Password = dto.Password, // Password will be hashed in the service
            UserName = dto.DisplayName
        };

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user.Id;
    }

    public async Task<DB.User> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }
}