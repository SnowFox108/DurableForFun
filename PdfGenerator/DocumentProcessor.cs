using System;
using System.Linq;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PdfGenerator.Infrastructure;

namespace PdfGenerator
{
    [StorageAccount("AzureWebJobsStorage")]
    public class DocumentProcessor
    {
        [FunctionName("DocumentProcessor")]
        [return: Queue("PdfTaskCompleteQueue")]
        public static string Run([TimerTrigger("*/1 * * * *")]TimerInfo myTimer,
            //[Table("PdfTask", "", Filter = "IsProcessed eq 'false'")] IEnumerable<PdfTaskEntity>,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var task = DataContext.Instance.PdfTaskEntities.FirstOrDefault(x => x.IsProcessed == false);

            if (task == null)
            {
                log.LogInformation($"There is no Pdf document waiting in the queue.");

                return null;
            }
            else
            {
                log.LogInformation($"Build Pdf document for {task.TradeId}");

                task.IsProcessed = true;
                var command = new PdfCommand()
                {
                    TradeId = task.TradeId,
                    OrchestrationId = task.OrchestrationId,
                    PdfPath = $"{task.TradeId}.pdf"
                };

                var message = JsonSerializer.Serialize(command);

                return message;
            }
            // Execute the query and loop through the results
            //foreach (PdfTaskEntity entity in entities)
            //{
            //    log.LogInformation(
            //        $"{entity.TradeId}\t{entity.IsProcessed}");
            //}

            //await Task.Delay(1000);
        }
    }
}
