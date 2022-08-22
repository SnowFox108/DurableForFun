using System;
using System.Text.Json;
using CloudNative.CloudEvents;
using Dapr.AzureFunctions.Extension;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace FenicsDispatcher
{
    [StorageAccount("AzureWebJobsStorage")]
    public class FenicsHandler
    {
        [FunctionName("FenicsHandler")]
        public void Run(
            //[QueueTrigger("FenicsTaskQueue")]string fenicsCommand, 
            [DaprTopicTrigger("redis-pubsub", Topic = "FenicsTask")] CloudEvent subEvent,

            ILogger log)
        {
            //var command = JsonSerializer.Deserialize<FenicsCommand>(fenicsCommand);
            var command = JsonSerializer.Deserialize<FenicsCommand>(subEvent.Data.ToString());

            log.LogInformation($"New Fenics task has been put in: Task {command.TradeId}");

            var account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process));
            var client = account.CreateCloudTableClient();

            var table = client.GetTableReference("FenicsTask");

            table.CreateIfNotExistsAsync();

            var entity = new FenicsTaskEntity
            {
                PartitionKey = "FenicsDispatcher",
                RowKey = Guid.NewGuid().ToString("N"),
                TradeId = command.TradeId,
                IsProcessed = false,
                OrchestrationId = command.OrchestrationId
            };

            TableOperation insertOperation = TableOperation.Insert(entity);
            table.ExecuteAsync(insertOperation);

        }
    }
}
