using BlogBE.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BlogBE.User;

public class ActivityLogService
{
    private readonly IMongoCollection<ActivityLog> _collection;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly ILogger<ActivityLogService>
        _logger; //This is not a business activity log, this is a system health log instead

    public ActivityLogService(IMongoCollection<ActivityLog> collection, IHttpContextAccessor http,
        IHttpContextAccessor httpContextAccessor, ILogger<ActivityLogService> logger)
    {
        _collection = collection;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task LogAsync(string eventType, int? userId = null, object? metadata = null)
    {
        var ctx = _httpContextAccessor.HttpContext;

        var log = new ActivityLog
        {
            UserId = userId,
            EventType = eventType,
            OccurredAt = DateTime.UtcNow,
            IpAddress = ctx?.Items["Ip"]?.ToString(),
            UserAgent = ctx?.Items["UserAgent"]?.ToString(),
            CorrelationId = ctx?.Items["CorrelationId"]?.ToString(),
            Metadata = metadata != null
                ? metadata.ToBsonDocument()
                : new BsonDocument()
        };
        _logger.LogInformation("User {User} performed {Action} at {Time}",
            userId ?? 0, eventType, log.OccurredAt);
        await _collection.InsertOneAsync(log);
    }
}