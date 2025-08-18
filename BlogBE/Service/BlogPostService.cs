using BlogBE.PostgreDb;
using Microsoft.EntityFrameworkCore;

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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.BlogPosts.Add(post);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> TryDeletePostAsync(int postId, int userId)
    {
        var post = await _dbContext.BlogPosts.FindAsync(postId);
        if (post == null || post.AuthorId != userId)
        {
            return false; // Post not found or user is not the author
        }

        _dbContext.BlogPosts.Remove(post);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryUpdatePostAsync(int postId, string title, string content, int userId)
    {
        var post = await _dbContext.BlogPosts.FindAsync(postId);
        if (post == null || post.AuthorId != userId)
        {
            return false; // Post not found or user is not the author
        }

        post.Title = title;
        post.Content = content;
        post.UpdatedAt = DateTime.UtcNow;

        _dbContext.BlogPosts.Update(post);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<BlogPost>> GetPostsAsync(int page, int pageSize)
    {
        return await _dbContext.BlogPosts
            .OrderByDescending(post => post.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<BlogPost>> GetPostsByUserIdAsync(int userId, int page, int pageSize)
    {
        return await _dbContext.BlogPosts
            .Where(post => post.AuthorId == userId)
            .OrderByDescending(post => post.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<bool> PostExistsAsync(int postId)
    {
        return await _dbContext.BlogPosts.AnyAsync(post => post.Id == postId);
    }
}