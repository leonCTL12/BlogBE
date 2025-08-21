using System.Security.Claims;
using BlogBE.Constants;
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
    private readonly ActivityLogService _activityLogService;
    private readonly BlogPostService _blogPostService;
    private readonly CommentPermissionService _commentPermissionService;
    private readonly CommentService _commentService;
    private readonly UserService _userService;

    public CommentController(CommentService commentService, UserService userService, BlogPostService blogPostService,
        CommentPermissionService commentPermissionService, ActivityLogService activityLogService)
    {
        _commentService = commentService;
        _userService = userService;
        _blogPostService = blogPostService;
        _commentPermissionService = commentPermissionService;
        _activityLogService = activityLogService;
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
        _ = _activityLogService.LogAsync(ActivityLogEvent.UserCreatedComment, userId.Value);
        return Ok(new { message = "Comment created successfully", content = requestDto.Content });
    }

    [HttpDelete("delete/{commentId}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var userId = await _userService.GetUserIdByClaimASync(User.FindFirst(ClaimTypes.NameIdentifier));
        if (userId == null)
        {
            return Unauthorized();
        }

        var comment = await _commentService.GetCommentByIdAsync(commentId);

        if (comment == null)
        {
            return NotFound(new { message = "Comment not found" });
        }

        var canDelete = await _commentPermissionService.CanUserDeleteComment(comment, userId.Value);
        if (!canDelete)
        {
            return Unauthorized();
        }

        var result = await _commentService.TryDeleteCommentById(commentId, comment.PostId);
        if (!result)
        {
            return NotFound(new { message = "Comment not found or you are not the author" });
        }

        _ = _activityLogService.LogAsync(ActivityLogEvent.UserDeletedComment, userId.Value);
        return Ok(new { message = "Comment deleted successfully" });
    }

    [HttpGet("get/{postId}")]
    public async Task<IActionResult> GetComments(int postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var postExists = await _blogPostService.PostExistsAsync(postId);
        if (!postExists)
        {
            return NotFound(new { message = "Post not found" });
        }

        var comments = await _commentService.GetCommentForPostAsync(postId, page, pageSize);
        return Ok(comments);
    }
}