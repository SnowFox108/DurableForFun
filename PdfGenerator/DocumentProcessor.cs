using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using PdfGenerator.Infrastructure;

namespace PdfGenerator
{
    [StorageAccount("AzureWebJobsStorage")]
    public class DocumentProcessor
    {
        [FunctionName("DocumentProcessor")]
        [return: Queue("PdfTaskCompleteQueue")]
        public async Task<string> Run([TimerTrigger("*/1 * * * *")]TimerInfo myTimer,
            //[Table("PdfTask", "", Filter = "IsProcessed eq 'false'")] IEnumerable<PdfTaskEntity>,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var condition = TableQuery.GenerateFilterConditionForBool("IsProcessed", QueryComparisons.Equal, false);

            var query = new TableQuery<PdfTaskEntity>().Where(condition);

            var account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var client = account.CreateCloudTableClient();

            var table = client.GetTableReference("PdfTask");

            var lst = await table.ExecuteQuerySegmentedAsync(query, null);
            var task = lst.FirstOrDefault();

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

                TableOperation updateOperation = TableOperation.Replace(task);
                await table.ExecuteAsync(updateOperation);

                //return message;
            }

            return null;
        }
    }
}
