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
            throw new ArgumentException("Invalid comment creation data.", nameof(requestDto));
        }

        var userId = await _userService.GetUserIdByClaimASync(User.FindFirst(ClaimTypes.NameIdentifier));
        if (userId == null)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var postExists = await _blogPostService.PostExistsAsync(requestDto.PostId);
        if (!postExists)
        {
            throw new KeyNotFoundException("Post not found.");
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
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var comment = await _commentService.GetCommentByIdAsync(commentId);

        if (comment == null)
        {
            throw new KeyNotFoundException("Comment not found.");
        }

        var canDelete = await _commentPermissionService.CanUserDeleteComment(comment, userId.Value);
        if (!canDelete)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this comment.");
        }

        var result = await _commentService.TryDeleteCommentById(commentId, comment.PostId);
        if (!result)
        {
            throw new KeyNotFoundException("Comment not found or already deleted.");
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
            throw new KeyNotFoundException("Post not found.");
        }

        var comments = await _commentService.GetCommentForPostAsync(postId, page, pageSize);
        return Ok(comments);
    }
}