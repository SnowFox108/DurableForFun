using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PdfGenerator.Infrastructure;

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

            DataContext.Instance.PdfTaskEntities.Add(new PdfTaskEntity
            {
                TradeId = command.TradeId,
                IsProcessed = false,
                OrchestrationId = command.OrchestrationId
            });

            //return new PdfTaskEntity
            //{
            //    PartitionKey = "PdfGenerator",
            //    RowKey = Guid.NewGuid().ToString("N"),
            //    TradeId = command.TradeId,
            //    IsProcessed= false,
            //    OrchestrationId = command.OrchestrationId
            //};

        }
    }
}
