using BlogBE.Constants;
using BlogBE.DTO;
using BlogBE.User;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogBE.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] //Do not need authentication for this controller
public class AuthController : ControllerBase
{
    private readonly ActivityLogService _activityLogService;
    private readonly UserService _userService;

    public AuthController(UserService userService, ActivityLogService activityLogService)
    {
        _userService = userService;
        _activityLogService = activityLogService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterRequest dto,
        [FromServices] IValidator<RegisterRequest> validator)
    {
        var result = await validator.ValidateAsync(dto);

        if (!result.IsValid) return BadRequest(result.Errors);


        var id = await _userService.RegisterAsync(dto);
        var createdUser = await _userService.GetUserByIdAsync(id);

        await _activityLogService.LogAsync(ActivityLogEvent.UserRegistered, createdUser.Id);

        // Return only one object with the desired properties in a JSON format
        return Ok(new
        {
            id = createdUser.Id,
            userName = createdUser.UserName,
            email = createdUser.Email
        });
    }
}