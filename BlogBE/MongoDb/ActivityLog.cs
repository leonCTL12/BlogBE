using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlogBE.MongoDb;

public class ActivityLog
{
    [BsonId] public ObjectId Id { get; set; }

    public int? UserId { get; set; }
    public string EventType { get; set; } = null!;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    public BsonDocument Metadata { get; set; } = new(); //Flexible event specific data without changing schema
    public string? UserAgent { get; set; } //i.e. device/browser info
    public string? IpAddress { get; set; }
    public string? CorrelationId { get; set; } //i.e. tie related logs together
    public bool? Success { get; set; }
}