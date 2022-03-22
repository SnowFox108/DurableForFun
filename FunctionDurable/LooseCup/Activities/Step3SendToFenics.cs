using System.Text.Json;
using System.Threading.Tasks;
using FunctionDurable.LooseCup.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FunctionDurable.LooseCup.Activities
{
    public static class Step3SendToFenicsActvity
    {
        [FunctionName("SendToFenics")]
        [return: Queue("FenicsTaskQueue")]
        public static async Task<string> SendToFenics([ActivityTrigger] FenicsCommand command, ILogger log)
        {
            log.LogWarning($"Send Trade {command.TradeId} to Fenics.");

            var message = JsonSerializer.Serialize(command);

            await Task.Delay(100);
            return message;

        }

        [FunctionName("FenicsComplete")]
        public static async Task FenicsComplete(
            [QueueTrigger("FenicsTaskCompleteQueue")] string fenicsCommand,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            var result = JsonSerializer.Deserialize<FenicsCommand>(fenicsCommand);

            log.LogWarning($"Mission Fenics: {result.TradeId} has completed.");

            await client.RaiseEventAsync(result.OrchestrationId, "FenicsTaskComplete", result.FenicsId);
        }

    }
}
