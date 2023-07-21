using MongoDB.Bson;
using MongoDB.Driver;
using Webley.Mongo.Projection.Shared;

namespace Webley.Mongo.Projection.Library;
public class TaskService
{
    private readonly IMongoCollection<TaskEntity> _taskCollection;

    public TaskService(IMongoCollection<TaskEntity> taskCollection)
    {
        _taskCollection = taskCollection;
    }

    public async Task InsertDummyData()
    {
        await _taskCollection.DeleteManyAsync(Builders<TaskEntity>.Filter.Empty);

        var tasksToInsert = new List<TaskEntity>
        {
            new TaskEntity { Name = "Task 1", Links = new string[2] {"link1", "link2"} },
            new TaskEntity { Name = "Task 2", Links = new string[4] {"link3", "link4", "link5", "link6"} },
            new TaskEntity { Name = "Task 3", Links = Array.Empty<string>() },
            new TaskEntity { Name = "Task 3", Links = null }
        };

        await _taskCollection.InsertManyAsync(tasksToInsert);
    }

    public async Task<List<TaskHeader>> GetTasksSimpleAggregation()
    {
        var projection = BuildProjectionFluent();
        return await SimpleAggregation(projection);
    }

    public async Task<List<TaskHeader>> GetTasksFacetedAggregation()
    {
        var projection = BuildProjectionFluent();
        return await FacetedAggregation(projection);
    }

    public async Task<List<TaskHeader>> GetTasksFacetedAggregationWithBsonProjection()
    {
        var projection = BuildProjectionBson();
        return await FacetedAggregation(projection);
    }

    private ProjectionDefinition<TaskEntity, TaskHeader> BuildProjectionFluent()
    {
        var projection = Builders<TaskEntity>.Projection.Expression(x =>
            new TaskHeader
            {
                Id = x.Id,
                Name = x.Name,
                LinkCount = x.Links != null ? x.Links.Length : 0
            });

        return projection;
    }

    private ProjectionDefinition<TaskEntity, TaskHeader> BuildProjectionBson()
    {
        var bsonProjection = new BsonDocumentProjectionDefinition<TaskEntity, TaskHeader>(
            BsonDocument.Parse(@"{
              Name: 1,
              LinkCount: {
                $cond: {
                  if: {
                    $isArray: ""$Links""
                  },
                  then: {
                    $size: ""$Links""
                  },
                  else: 0
                }
              }
            }"
        ));

        return bsonProjection;
    }

    private async Task<List<TaskHeader>> SimpleAggregation(
            ProjectionDefinition<TaskEntity, TaskHeader>? projection,
            CancellationToken ct = default)
    {
        var projectionStage = PipelineStageDefinitionBuilder.Project(projection);
        var aggregation = _taskCollection.Aggregate().AppendStage(projectionStage);
        var results = await aggregation.ToListAsync();
        return results;
    }

    private async Task<List<TaskHeader>> FacetedAggregation(
            ProjectionDefinition<TaskEntity, TaskHeader>? projection,
            CancellationToken ct = default)
    {
        var countFacet = AggregateFacet.Create("count",
            PipelineDefinition<TaskEntity, AggregateCountResult>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Count<TaskEntity>()
            }));

        var dataFacet = AggregateFacet.Create("data",
            PipelineDefinition<TaskEntity, TaskHeader>.Create(new IPipelineStageDefinition[]
            {
                PipelineStageDefinitionBuilder.Skip<TaskEntity>(1),
                PipelineStageDefinitionBuilder.Project(projection)
            }));

        var facet = _taskCollection.Aggregate().Facet(countFacet, dataFacet);
        var facetResults = await facet.SingleAsync(ct);

        var data = facetResults.Facets.First(x => x.Name == "data").Output<TaskHeader>();
        var count = facetResults.Facets.First(x => x.Name == "count").Output<AggregateCountResult>().Single().Count;

        return data.ToList();
    }
}
