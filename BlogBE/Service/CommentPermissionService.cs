using BlogBE.PostgreDb;
using BlogBE.User;

namespace BlogBE.Service;

public class CommentPermissionService
{
    private readonly BlogPostService _blogPostService;

    public CommentPermissionService(BlogPostService blogPostService)
    {
        _blogPostService = blogPostService;
    }

    public async Task<bool> CanUserDeleteComment(Comment comment, int userId)
    {
        if (comment.AuthorId == userId)
        {
            return true;
        }

        var isPostAuthor = await _blogPostService.UserIsAuthorAsync(comment.PostId, userId);
        return isPostAuthor;
    }
}