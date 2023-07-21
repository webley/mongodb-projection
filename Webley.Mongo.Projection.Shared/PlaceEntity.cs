using MongoDB.Driver.GeoJsonObjectModel;

namespace Webley.Mongo.Projection.Shared;

public class PlaceEntity
{
    public string? Name { get; set; }
    public GeoJson2DCoordinates? Location { get; set; }
}
