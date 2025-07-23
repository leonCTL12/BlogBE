using BlogBE.User;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BlogBE.Controllers;

[ApiController]
[Route("[controller]")]
public class UserManagementController: ControllerBase
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
        var createdUser = await _userService.GetUserByIdAsync(user.Id);

        // Return only one object with the desired properties in a JSON format
        return Ok(new 
        { 
            id = createdUser.Id, 
            userName = createdUser.UserName, 
            email = createdUser.Email 
        });
    }
}