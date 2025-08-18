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
public class CommentController : ControllerBase
{
    private readonly BlogPostService _blogPostService;
    private readonly CommentService _commentService;
    private readonly UserService _userService;

    public CommentController(CommentService commentService, UserService userService, BlogPostService blogPostService)
    {
        _commentService = commentService;
        _userService = userService;
        _blogPostService = blogPostService;
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequestDto requestDto,
        [FromServices] IValidator<CreateCommentRequestDto> validator)
    {
        var validationResult = await validator.ValidateAsync(requestDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var userId = await _userService.GetUserIdByClaimASync(User.FindFirst(ClaimTypes.NameIdentifier));
        if (userId == null)
        {
            return Unauthorized();
        }

        var postExists = await _blogPostService.PostExistsAsync(requestDto.PostId);
        if (!postExists)
        {
            return NotFound(new { message = "Post not found" });
        }

        await _commentService.CreateCommentAsync(requestDto.Content, requestDto.PostId, userId.Value);

        return Ok(new { message = "Comment created successfully", content = requestDto.Content });
    }
}