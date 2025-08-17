using BlogBE.PostgreDb;

namespace BlogBE.User;

public class BlogPostService
{
    private readonly BlogDbContext _dbContext;

    public BlogPostService(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreatePostAsync(string title, string content, int userId)
    {
        var post = new BlogPost
        {
            Title = title,
            Content = content,
            AuthorId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.BlogPosts.Add(post);
        await _dbContext.SaveChangesAsync();
    }
}