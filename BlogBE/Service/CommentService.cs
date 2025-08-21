using BlogBE.PostgreDb;
using Microsoft.EntityFrameworkCore;

namespace BlogBE.Service;

public class CommentService
{
    private readonly BlogDbContext _dbContext;
    private readonly RedisCacheService _redisCacheService;

    public CommentService(BlogDbContext dbContext, RedisCacheService redisCacheService)
    {
        _dbContext = dbContext;
        _redisCacheService = redisCacheService;
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
        _ = _redisCacheService.InvalidateCommentsCacheAsync(postId);
    }

    public async Task<bool> TryDeleteCommentById(int commentId, int postId)
    {
        var comment = await _dbContext.Comments.FindAsync(commentId);
        if (comment == null)
        {
            return false;
        }

        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync();
        _ = _redisCacheService.InvalidateCommentsCacheAsync(postId);
        return true;
    }

    public async Task<Comment?> GetCommentByIdAsync(int commentId)
    {
        return await _dbContext.Comments.FindAsync(commentId);
    }


    public async Task<List<Comment>> GetCommentForPostAsync(int postId, int page, int pageSize)
    {
        var cachedComments = await _redisCacheService.GetCommentsAsync(postId, page, pageSize);
        if (cachedComments != null)
        {
            return cachedComments;
        }

        var comments = await _dbContext.Comments
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        _ = _redisCacheService.CacheCommentsAsync(postId, comments, page, pageSize);
        return comments;
    }
}