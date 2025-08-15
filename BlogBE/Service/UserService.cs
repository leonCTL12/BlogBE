using BlogBE.Data;
using BlogBE.DTO;
using Microsoft.EntityFrameworkCore;

namespace BlogBE.User;

public class UserService
{
    private readonly BlogDbContext _dbContext;

    public UserService(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
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
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user.Id;
    }

    public async Task<DB.User?> GetUserByIdAsync(int id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<DB.User?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> VerifyPasswordAsync(DB.User user, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, user.Password);
    }
}