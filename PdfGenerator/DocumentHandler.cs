using System;
using System.Text.Json;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Dapr.AzureFunctions.Extension;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace PdfGenerator
{
    [StorageAccount("AzureWebJobsStorage")]
    public class DocumentHandler
    {
        [FunctionName("DocumentHandler")]
        //[return: Table("PdfTask")]
        public static async Task Run(
            //[QueueTrigger("PdfTaskQueue")]string pdfCommand, 
            [DaprTopicTrigger("redis-pubsub", Topic = "PdfTask")] CloudEvent subEvent,
            ILogger log)
        {
            log.LogInformation($"Message received {subEvent.Data}");

            //var command = JsonSerializer.Deserialize<PdfCommand>(pdfCommand);
            var command = JsonSerializer.Deserialize<PdfCommand>(subEvent.Data.ToString());

            log.LogInformation($"New task has been put in: Task {command.TradeId}");

            var account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process));
            var client = account.CreateCloudTableClient();

            var table = client.GetTableReference("PdfTask");

            table.CreateIfNotExistsAsync();

            var entity = new PdfTaskEntity
            {
                PartitionKey = "PdfGenerator",
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
