using MongoDB.Bson.Serialization.Attributes;

namespace Webley.Mongo.Projection.Shared;

public class TaskEntity
{
    [BsonId]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string[]? Links { get; set; }
}
