using BlogBE.Constants;
using BlogBE.DTO;
using BlogBE.Jwt;
using BlogBE.Service;
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
    private readonly JwtTokenFactory _jwtTokenFactory;
    private readonly UserService _userService;

    public AuthController(UserService userService, ActivityLogService activityLogService,
        JwtTokenFactory jwtTokenFactory)
    {
        _userService = userService;
        _activityLogService = activityLogService;
        _jwtTokenFactory = jwtTokenFactory;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterRequest dto,
        [FromServices] IValidator<RegisterRequest> validator)
    {
        var result = await validator.ValidateAsync(dto);

        if (!result.IsValid)
        {
            return BadRequest(result.Errors);
        }


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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var user = await _userService.GetUserByEmailAsync(dto.Email);
        if (user == null)
        {
            await _activityLogService.LogAsync(ActivityLogEvent.UserLoggedInFailed, null,
                new { email = dto.Email, reason = "User not found" });
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var isPasswordValid = await _userService.VerifyPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
        {
            await _activityLogService.LogAsync(ActivityLogEvent.UserLoggedInFailed, user.Id,
                new { email = dto.Email, reason = "Invalid password" });
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var (token, expiresAt) = _jwtTokenFactory.CreateToken(user);

        await _activityLogService.LogAsync(ActivityLogEvent.UserLoggedIn, user.Id,
            new { email = dto.Email, tokenExpiration = expiresAt });
        return Ok(new LoginResponseDto
        (
            user.Id,
            user.UserName,
            user.Email,
            token,
            expiresAt
        ));
    }
}