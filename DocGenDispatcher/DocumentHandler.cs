using System.Text.Json;
using System.Threading.Tasks;
using DocGenDispatcher;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace PdfGenerator
{
    [StorageAccount("AzureWebJobsStorage")]
    public class DocumentHandler
    {
        [FunctionName("DocumentHandler")]
        [return: Queue("DocGenTaskCompleteQueue")]
        public static async Task<string> Run([QueueTrigger("DocGenTaskQueue")]string docGenCommand, 
            ILogger log)
        {
            var command = JsonSerializer.Deserialize<DocGenCommand>(docGenCommand);

            log.LogInformation($"New DocGen task has been put in: Task {command.TradeId}");

            await Task.Delay(1000);
            var message = JsonSerializer.Serialize(command);

            return message;

        }
    }
}
