using BlogBE.PostgreDb;
using Microsoft.EntityFrameworkCore;

namespace BlogBE.Service;

public class CommentService
{
    private readonly BlogDbContext _dbContext;

    public CommentService(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateCommentAsync(string content, int postId, int authorId)
    {
        var comment = new Comment
        {
            Content = content,
            CreatedAt = DateTime.UtcNow,
            PostId = postId,
            AuthorId = authorId
        };

        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteCommentAsync(int commentId, int userId)
    {
        var comment = await _dbContext.Comments.FindAsync(commentId);
        if (comment == null || comment.AuthorId != userId)
        {
            return false; // Comment not found or user is not the author
        }

        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<Comment>> GetCommentForPostAsync(int postId, int page, int pageSize)
    {
        return await _dbContext.Comments
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}