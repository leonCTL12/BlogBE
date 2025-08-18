using System.Security.Claims;
using BlogBE.DTO;
using BlogBE.Service;
using BlogBE.User;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogBE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogPostController : ControllerBase
{
    private readonly BlogPostService _blogPostService;
    private readonly UserService _userService;

    public BlogPostController(UserService userService, BlogPostService blogPostService)
    {
        _userService = userService;
        _blogPostService = blogPostService;
    }

    private async Task<int?> GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            return null;
        }

        if (!int.TryParse(claim.Value, out var userId))
        {
            return null;
        }


        if (!await _userService.UserExistsAsync(userId))
        {
            return null;
        }

        return userId;
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequestDto requestDto,
        [FromServices] IValidator<CreatePostRequestDto> validator)
    {
        var validationResult = await validator.ValidateAsync(requestDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var userId = await GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        await _blogPostService.CreatePostAsync(requestDto.Title, requestDto.Content, userId.Value);

        // Return a success response
        return Ok(new { message = "Post created successfully", title = requestDto.Title });
    }

    [HttpDelete("delete/{postId}")]
    [Authorize]
    public async Task<IActionResult> DeletePost(int postId)
    {
        var userId = await GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _blogPostService.TryDeletePostAsync(postId, userId.Value);

        if (!result)
        {
            return NotFound(new { message = "Post not found or you are not the author." });
        }

        return Ok(new { message = "Post deleted successfully", postId });
    }

    [HttpPut("update/{postId}")]
    [Authorize]
    public async Task<IActionResult> UpdatePost(int postId, [FromBody] UpdatePostRequestDto requestDto,
        [FromServices] IValidator<UpdatePostRequestDto> validator)
    {
        var userId = await GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var validationResult = await validator.ValidateAsync(requestDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        if (!await _blogPostService.TryUpdatePostAsync(postId, requestDto.Title, requestDto.Content, userId.Value))
        {
            return NotFound(new { message = "Post not found or you are not the author." });
        }

        return Ok(new { message = "Post updated successfully", postId, title = requestDto.Title });
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var posts = await _blogPostService.GetPostsAsync(page, pageSize);
        return Ok(posts);
    }
}