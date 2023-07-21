using MongoDB.Driver;
using Webley.Mongo.Projection.Shared;

namespace Webley.Mongo.Projection.Library;
public static class LinqProjectionRunner
{
    public async static Task Run(IMongoDatabase db)
    {
        var taskCollection = db.GetCollection<TaskEntity>("tasks");

        var service = new TaskService(taskCollection);
        await service.InsertDummyData();

        Console.WriteLine("Calling simple aggregation with fluent projection... ");
        try
        {
            // Returns 4 results
            var fluentResults = await service.GetTasksSimpleAggregation();
            Console.WriteLine($"Results returned: {fluentResults.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception thrown: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Calling faceted aggregation with BSON document projection... ");
        try
        {
            // Returns 4 results
            var bsonResults = await service.GetTasksFacetedAggregationWithBsonProjection();
            Console.WriteLine($"Results returned: {bsonResults.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception thrown: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Calling faceted aggregation with fluent projection... ");
        try
        {
            // Exception thrown: Element 'Links' does not match any field or property of class Webley.Mongo.Projection.Console.TaskHeader.
            var bsonResults = await service.GetTasksFacetedAggregation();
            Console.WriteLine($"Results returned: {bsonResults.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception thrown: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Calling faceted aggregation within the class library... ");
        try
        {
            // Exception thrown: Element 'Links' does not match any field or property of class Webley.Mongo.Projection.Console.TaskHeader.
            var bsonResults = await service.GetTasksFacetedAggregation();
            Console.WriteLine($"Results returned: {bsonResults.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception thrown: {ex.Message}");
        }
    }
}
