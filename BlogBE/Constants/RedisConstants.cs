namespace BlogBE.Constants;

public static class RedisConstants
{
    public const int ExpirationTimeInSeconds = 60;
    public const string AllPostsKey = "all_posts";
    public const string UserPostsKeyPrefix = "user_posts_";
    public const string CacheVersionKey = "cache_version";
}