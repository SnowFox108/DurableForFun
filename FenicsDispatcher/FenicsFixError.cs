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
using System.Collections.Generic;

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

            //var condition1 = TableQuery.GenerateFilterConditionForBool("IsProcessed", QueryComparisons.Equal, false);
            var condition = TableQuery.GenerateFilterConditionForInt("TradeId", QueryComparisons.Equal, tradeId);

            //var condition = TableQuery.CombineFilters(condition1, TableOperators.And, condition2);

            var query = new TableQuery<FenicsTaskEntity>().Where(condition);

            var account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var client = account.CreateCloudTableClient();

            var table = client.GetTableReference("FenicsTask");

            var lst = new List<FenicsTaskEntity>();
            TableQuerySegment<FenicsTaskEntity> segment = null;

            while (segment == null || segment.ContinuationToken != null)
            {
                segment = await table.ExecuteQuerySegmentedAsync(query, segment?.ContinuationToken);
                lst.AddRange(segment.Results);
            }

            var task = lst.FirstOrDefault(t => t.IsProcessed == false);


            if (task == null)
            {
                log.LogInformation($"There is no TradeId: {tradeId} in the queue.");

                return null;
            }
            else
            {
                log.LogInformation($"Create Fenics for {task.TradeId}");

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

                return message;
            }

        }
    }
}
