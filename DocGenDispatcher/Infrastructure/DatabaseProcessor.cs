using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace PdfGenerator.Infrastructure;

public class DatabaseProcessor
{
    private readonly TaskHubEventTrackingQuery _eventTrackingQuery;

    public DatabaseProcessor()
    {
        var connectionFactory = new ConnectionFactory();
        _eventTrackingQuery = new TaskHubEventTrackingQuery(connectionFactory);
    }

    public async Task ProcessTask(ILogger log)
    {
        var id = "e5034b0a-4f94-435e-bbea-633c3af2d72d";
        var results = await _eventTrackingQuery.GetTaskByOrchestrationId(id);

        foreach (var result in results)
        {
            log.LogInformation($"Get Task Id: {result.Id}");
        }
    }

}