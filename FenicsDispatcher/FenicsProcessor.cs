using FenicsDispatcher.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;

namespace FenicsDispatcher
{
    [StorageAccount("AzureWebJobsStorage")]

    public class FenicsProcessor
    {
        [FunctionName("FenicsProcessor")]
        [return: Queue("FenicsTaskCompleteQueue")]
        public static string Run([TimerTrigger("*/1 * * * *")] TimerInfo myTimer,
            //[Table("PdfTask", "", Filter = "IsProcessed eq 'false'")] IEnumerable<PdfTaskEntity>,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var task = DataContext.Instance.FenicsEntities.FirstOrDefault(x => x.IsProcessed == false);

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
