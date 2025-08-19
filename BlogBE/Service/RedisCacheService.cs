using BlogBE.Constants;
using BlogBE.PostgreDb;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BlogBE.Service;

public class RedisCacheService
{
    private readonly IDatabase _redis;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _redis = connectionMultiplexer.GetDatabase();
    }

    public async Task<List<BlogPost>?> GetPostsAsync()
    {
        var cachedString = await _redis.StringGetAsync(RedisConstants.AllPostsKey);
        if (cachedString.IsNullOrEmpty)
        {
            return null; // Cache miss
        }

        var posts = JsonConvert.DeserializeObject<List<BlogPost>>(cachedString);
        return posts;
    }

    public Task CacheBlogPost(List<BlogPost> posts)
    {
        var serializedPosts = JsonConvert.SerializeObject(posts);
        return _redis.StringSetAsync(RedisConstants.AllPostsKey, serializedPosts,
            TimeSpan.FromSeconds(RedisConstants.ExpirationTimeInSeconds));
    }

    public Task InvalidatePostsCacheAsync()
    {
        return _redis.KeyDeleteAsync(RedisConstants.AllPostsKey);
    }
}