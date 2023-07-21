using MongoDB.Driver;
using Webley.Mongo.Projection.Shared;

namespace Webley.Mongo.Projection.Library;

public class PlaceService
{
    private readonly IMongoCollection<PlaceEntity> _places;

	public PlaceService(IMongoCollection<PlaceEntity> places)
	{
		_places = places;
	}

	public async Task<List<PlaceEntity>> GetPlaceEntities()
	{
		return await _places.Find(Builders<PlaceEntity>.Filter.Empty).ToListAsync();
	}

    public async Task<List<PlaceHeader>> GetPlaceHeaders()
    {
        return await _places
			.Find(Builders<PlaceEntity>.Filter.Empty)
			.Project(x => new PlaceHeader
			{
				Name = x.Name,
				Latitude = x.Location != null ? x.Location.X : null,
                Longitude = x.Location != null ? x.Location.Y : null,
            })
			.ToListAsync();
    }
}
