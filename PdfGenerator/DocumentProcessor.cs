using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

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

            var result = await TaskProcessor(log);
            return result;
        }

        [FunctionName("DocumentBuilder")]
        [return: Queue("PdfTaskCompleteQueue")]
        public async Task<string> Builder(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,

            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var result = await TaskProcessor(log);
            return result;
        }

        private async Task<string> TaskProcessor(ILogger log)
        {
            var condition = TableQuery.GenerateFilterConditionForBool("IsProcessed", QueryComparisons.Equal, false);

            var query = new TableQuery<PdfTaskEntity>().Where(condition);

            var account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process));
            var client = account.CreateCloudTableClient();

            var table = client.GetTableReference("PdfTask");
            await table.CreateIfNotExistsAsync();

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

                return message;
            }
        }
    }
}
