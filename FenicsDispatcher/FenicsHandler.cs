using System.Text.Json;
using FenicsDispatcher.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FenicsDispatcher
{
    [StorageAccount("AzureWebJobsStorage")]
    public class FenicsHandler
    {
        [FunctionName("FenicsHandler")]
        public void Run([QueueTrigger("FenicsTaskQueue")]string fenicsCommand, ILogger log)
        {
            var command = JsonSerializer.Deserialize<FenicsCommand>(fenicsCommand);

            log.LogInformation($"New Fenics task has been put in: Task {command.TradeId}");

            DataContext.Instance.FenicsEntities.Add(new FenicsEntity
            {
                TradeId = command.TradeId,
                IsProcessed = false,
                OrchestrationId = command.OrchestrationId
            });
        }
    }
}
