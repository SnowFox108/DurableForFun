using FenicsDispatcher.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FenicsDispatcher
{
    [StorageAccount("AzureWebJobsStorage")]

    public class FenicsProcessor
    {
        [FunctionName("FenicsProcessor")]
        [return: Queue("FenicsTaskCompleteQueue")]
        public async Task<string> Run([TimerTrigger("*/1 * * * *")] TimerInfo myTimer,
            //[Table("PdfTask", "", Filter = "IsProcessed eq 'false'")] IEnumerable<PdfTaskEntity>,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var result = await TaskProcessor(log);
            return result;
        }

        [FunctionName("FenicsBuilder")]
        [return: Queue("FenicsTaskCompleteQueue")]
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

            var query = new TableQuery<FenicsTaskEntity>().Where(condition);

            var account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var client = account.CreateCloudTableClient();

            var table = client.GetTableReference("FenicsTask");
            await table.CreateIfNotExistsAsync();

            var lst = await table.ExecuteQuerySegmentedAsync(query, null);
            var task = lst.FirstOrDefault();

            if (task == null)
            {
                log.LogInformation($"There is no task waiting in the queue.");

                return null;
            }
            else
            {
                log.LogInformation($"Create Fenics for {task.TradeId}");

                if (task.TradeId > 1 && task.TradeId < 4)
                {
                    log.LogWarning($"Something is wrong, fix it manually at: http://localhost:7076/api/FenicsFixError?TradeId={task.TradeId}");
                    return null;
                }

                task.IsProcessed = true;
                var dataContext = new DataContext();
                task.FenicsId = await dataContext.NextId();
                var command = new FenicsCommand()
                {
                    TradeId = task.TradeId,
                    OrchestrationId = task.OrchestrationId,
                    FenicsId = task.FenicsId
                };

                var message = JsonSerializer.Serialize(command);

                TableOperation updateOperation = TableOperation.Replace(task);
                await table.ExecuteAsync(updateOperation);

                log.LogInformation($"Fenics Job done for {task.TradeId}, sending back Fenics Id {task.FenicsId}");

                return message;
            }
        }

    }
}
