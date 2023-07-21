using MongoDB.Driver;
using Webley.Mongo.Projection.Library;

var settings = MongoClientSettings.FromConnectionString("mongodb://localhost:27017");
settings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V3;
var client = new MongoClient(settings);
var db = client.GetDatabase("test");

//await LinqProjectionRunner.Run(db);
await GeoJsonLinqProjectionRunner.Run(db);