using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FenicsDispatcher.Infrastructure;
using System.Linq;
using System.Text.Json;

namespace FenicsDispatcher
{
    public static class FenicsFixError
    {
        [FunctionName("FenicsFixError")]
        [return: Queue("FenicsTaskCompleteQueue")]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,

            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var tradeId = int.Parse(req.Query["TradeId"]);

            var task = DataContext.Instance.FenicsEntities.FirstOrDefault(x => x.TradeId == tradeId);

            if (task == null)
            {
                log.LogInformation($"There is no TradeId: {tradeId} in the queue.");

                return null;
            }
            else
            {
                log.LogInformation($"Create Fenics for {task.TradeId}");

                task.IsProcessed = true;
                var command = new FenicsCommand()
                {
                    TradeId = task.TradeId,
                    OrchestrationId = task.OrchestrationId,
                    FenicsId = DataContext.Instance.NextId()
                };

                var message = JsonSerializer.Serialize(command);

                return message;
            }

        }
    }
}
