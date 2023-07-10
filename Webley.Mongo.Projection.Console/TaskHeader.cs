using MongoDB.Bson.Serialization.Attributes;

namespace Webley.Mongo.Projection.Console;

public class TaskHeader
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int LinkCount { get; set; }
}
