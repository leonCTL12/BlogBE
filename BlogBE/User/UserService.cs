namespace BlogBE.User;

public class UserService
{
    public async Task<User> RegisterAsync(User user)
    {
        Console.WriteLine($"Creating user: {user.UserName}, Email: {user.Email}");
        
        return user;
    }
}