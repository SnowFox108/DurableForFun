using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FenicsDispatcher.Infrastructure;
using System.Linq;
using System.Text.Json;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Threading.Tasks;

namespace FenicsDispatcher
{
    public class FenicsFixError
    {
        [FunctionName("FenicsFixError")]
        [return: Queue("FenicsTaskCompleteQueue")]
        public async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,

            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var tradeId = int.Parse(req.Query["TradeId"]);

            var condition = TableQuery.GenerateFilterConditionForInt("TradeId", QueryComparisons.Equal, tradeId);

            var query = new TableQuery<FenicsTaskEntity>().Where(condition);

            var account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var client = account.CreateCloudTableClient();

            var table = client.GetTableReference("FenicsTask");

            var lst = await table.ExecuteQuerySegmentedAsync(query, null);
            var task = lst.FirstOrDefault();


            if (task == null)
            {
                log.LogInformation($"There is no TradeId: {tradeId} in the queue.");

                return null;
            }
            else
            {
                log.LogInformation($"Create Fenics for {task.TradeId}");

                task.IsProcessed = true;
                task.FenicsId = DataContext.Instance.NextId();
                var command = new FenicsCommand()
                {
                    TradeId = task.TradeId,
                    OrchestrationId = task.OrchestrationId,
                    FenicsId = task.FenicsId
                };

                var message = JsonSerializer.Serialize(command);

                TableOperation updateOperation = TableOperation.Replace(task);
                await table.ExecuteAsync(updateOperation);

                return message;
            }

        }
    }
}
