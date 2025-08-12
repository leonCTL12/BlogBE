using BlogBE.DTO;
using BlogBE.MongoDb;
using BlogBE.User;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BlogBE.Controllers;

[ApiController]
[Route("[controller]")]
public class UserManagementController : ControllerBase
{
    private readonly ActivityLogService _activityLogService;
    private readonly UserService _userService;

    public UserManagementController(UserService userService, ActivityLogService activityLogService)
    {
        _userService = userService;
        _activityLogService = activityLogService;
    }

    [HttpPost(Name = "CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterUserDto dto,
        [FromServices] IValidator<RegisterUserDto> validator)
    {
        var result = await validator.ValidateAsync(dto);

        if (!result.IsValid) return BadRequest(result.Errors);

        var user = new DB.User
        {
            Email = dto.Email,
            Password = dto.Password, // Password will be hashed in the service
            UserName = dto.DisplayName
        };
        await _userService.RegisterAsync(user);
        var createdUser = await _userService.GetUserByIdAsync(user.Id);

        await _activityLogService.LogAsync(new ActivityLog
        {
            UserId = createdUser.Id,
            EventType = "UserRegistered"
        });

        // Return only one object with the desired properties in a JSON format
        return Ok(new
        {
            id = createdUser.Id,
            userName = createdUser.UserName,
            email = createdUser.Email
        });
    }
}