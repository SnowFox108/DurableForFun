using System;
using System.Text.Json;
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
        public static void Run([QueueTrigger("PdfTaskQueue")]string pdfCommand, 
            ILogger log)
        {
            var command = JsonSerializer.Deserialize<PdfCommand>(pdfCommand);

            log.LogInformation($"New task has been put in: Task {command.TradeId}");

            var account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
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
