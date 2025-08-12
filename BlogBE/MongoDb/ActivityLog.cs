using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlogBE.MongoDb;

public class ActivityLog
{
    [BsonId] public ObjectId Id { get; set; }

    public int? UserId { get; set; }
    public string EventType { get; set; } = null!;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}