using BlogBE.Constants;
using BlogBE.General;
using BlogBE.PostgreDb;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BlogBE.Service;

public class RedisCacheService : IInitializable
{
    private const int MaxVersion = 1000000;
    private readonly IDatabase _redis;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _redis = connectionMultiplexer.GetDatabase();
    }

    public async Task InitializeAsync()
    {
        // this is to prevent the serving stale data from the cache when the app is restarted
        if (!await _redis.KeyExistsAsync(RedisConstants.CacheVersionKey))
        {
            await _redis.StringSetAsync(RedisConstants.CacheVersionKey, 1);
        }
    }

    public async Task<List<BlogPost>?> GetAllPostsAsync(int page, int pageSize)
    {
        try
        {
            var cacheVersionString = await _redis.StringGetAsync(RedisConstants.CacheVersionKey);
            var cacheKey = $"{RedisConstants.AllPostsKey}:{page}:{pageSize}:{cacheVersionString}";
            var cachedString = await _redis.StringGetAsync(cacheKey);
            if (cachedString.IsNullOrEmpty)
            {
                return null; // Cache miss
            }

            var posts = JsonConvert.DeserializeObject<List<BlogPost>>(cachedString!);
            return posts;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving posts from cache: {ex.Message}");
            return null; // Return null in case of any error
        }
    }

    public async Task CacheAllBlogPostAsync(List<BlogPost> posts, int page, int pageSize)
    {
        var cacheVersionString = await _redis.StringGetAsync(RedisConstants.CacheVersionKey);
        var cacheKey = $"{RedisConstants.AllPostsKey}:{page}:{pageSize}:{cacheVersionString}";
        var serializedPosts = JsonConvert.SerializeObject(posts);
        await _redis.StringSetAsync(cacheKey, serializedPosts,
            TimeSpan.FromSeconds(RedisConstants.ExpirationTimeInSeconds));
    }

    public async Task InvalidatePostsCacheAsync(int userId)
    {
        //Store the cache version in Redis instead of in-memory to ensure consistency under distributed (multi-instance) scenarios
        var cacheVersionString = await _redis.StringGetAsync(RedisConstants.CacheVersionKey);
        var newCacheVersion = int.TryParse(cacheVersionString, out var version) ? version + 1 : 1;
        if (newCacheVersion > MaxVersion)
        {
            newCacheVersion = 1; // Reset to 1 if it exceeds the maximum version
        }

        await _redis.StringSetAsync(RedisConstants.CacheVersionKey, newCacheVersion);

        //For per user cache invalidation, we can delete all keys that match the user ID pattern
        var userPostsPattern = $"{RedisConstants.UserPostsKeyPrefix}{userId}:*";
        var server = _redis.Multiplexer.GetServer(_redis.Multiplexer.GetEndPoints().First());
        var keys = server.Keys(pattern: userPostsPattern).ToArray();
        if (keys.Length > 0)
        {
            await _redis.KeyDeleteAsync(keys);
        }
    }

    public async Task<List<BlogPost>?> GetPostsByUserIdAsync(int userId, int page, int pageSize)
    {
        try
        {
            var cacheKey = $"{RedisConstants.UserPostsKeyPrefix}{userId}:{page}:{pageSize}";
            var cachedString = await _redis.StringGetAsync(cacheKey);
            if (cachedString.IsNullOrEmpty)
            {
                return null; // Cache miss
            }

            var posts = JsonConvert.DeserializeObject<List<BlogPost>>(cachedString!);
            return posts;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user posts from cache: {ex.Message}");
            return null; // Return null in case of any error
        }
    }

    public async Task CacheUserPostsAsync(int userId, List<BlogPost> posts, int page, int pageSize)
    {
        var cacheKey = $"{RedisConstants.UserPostsKeyPrefix}{userId}:{page}:{pageSize}";
        var serializedPosts = JsonConvert.SerializeObject(posts);
        await _redis.StringSetAsync(cacheKey, serializedPosts,
            TimeSpan.FromSeconds(RedisConstants.ExpirationTimeInSeconds));
    }
}