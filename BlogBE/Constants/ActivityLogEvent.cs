namespace BlogBE.Constants;

public static class ActivityLogEvent
{
    public const string UserRegistered = "UserRegistered";
    public const string UserLoggedIn = "UserLoggedIn";
    public const string UserLoggedInFailed = "UserLoggedInFailed";
    public const string UserCreatedPost = "UserCreatedPost";
    public const string UserUpdatedPost = "UserUpdatedPost";
    public const string UserDeletedPost = "UserDeletedPost";
    public const string UserCreatedComment = "UserCreatedComment";
    public const string UserDeletedComment = "UserDeletedComment";
}