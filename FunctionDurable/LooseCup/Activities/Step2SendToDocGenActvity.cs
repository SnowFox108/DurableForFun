using System.Text.Json;
using System.Threading.Tasks;
using FunctionDurable.LooseCup.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FunctionDurable.LooseCup.Activities
{
    public static class Step2SendToDocGenActvity
    {
        [FunctionName("SendToDocGen")]
        [return: Queue("DocGenTaskQueue")]
        public static async Task<string> SendToDocGen([ActivityTrigger] DocGenCommand command, ILogger log)
        {
            log.LogWarning($"Send email message to DocGen for Trade {command.TradeId}.");

            var message = JsonSerializer.Serialize(command);

            await Task.Delay(100);
            return message;
        }

        [FunctionName("DocGenComplete")]
        public static async Task GenerateDocumentComplete(
            [QueueTrigger("DocGenTaskCompleteQueue")] string docGenCommand,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            var result = JsonSerializer.Deserialize<DocGenCommand>(docGenCommand);

            log.LogWarning($"Mission DocGen dispatch: {result.TradeId} has completed.");

            await client.RaiseEventAsync(result.OrchestrationId, "DocGenTaskComplete");
        }

    }
}
