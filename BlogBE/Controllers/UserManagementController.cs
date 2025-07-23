using BlogBE.User;
using Microsoft.AspNetCore.Mvc;

namespace BlogBE.Controllers;

[ApiController]
[Route("[controller]")]
public class UserManagementController
{
    private readonly UserService _userService;

    public UserManagementController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost(Name = "CreateUser")] 
    public async Task<IActionResult> CreateUser([FromBody] User.User user)
    {
        await _userService.RegisterAsync(user);
        return new OkObjectResult(user);
    }
}