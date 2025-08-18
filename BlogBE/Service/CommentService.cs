using BlogBE.PostgreDb;

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
}