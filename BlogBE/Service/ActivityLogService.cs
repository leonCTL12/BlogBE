using BlogBE.MongoDb;
using MongoDB.Driver;

namespace BlogBE.User;

public class ActivityLogService
{
    private readonly IMongoCollection<ActivityLog> _collection;

    public ActivityLogService(IMongoCollection<ActivityLog> collection)
    {
        _collection = collection;
    }

    public async Task LogAsync(ActivityLog log)
    {
        await _collection.InsertOneAsync(log);
    }
}