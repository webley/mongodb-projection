using MongoDB.Driver;
using Webley.Mongo.Projection.Shared;

namespace Webley.Mongo.Projection.Library;
public class GeoJsonLinqProjectionRunner
{
    public async static Task Run(IMongoDatabase db)
    {
        var placeCollection = db.GetCollection<PlaceEntity>("places");

        var service = new PlaceService(placeCollection);

        try
        {
            Console.WriteLine("Getting place entities...");
            var entities = await service.GetPlaceEntities();
            Console.WriteLine($"Success. {entities.Count} places found.");

            Console.WriteLine("Getting place entities...");
            var headers = await service.GetPlaceHeaders();
            Console.WriteLine($"Success. {entities.Count} places found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception thrown: {ex.Message}");
        }        
    }
}
