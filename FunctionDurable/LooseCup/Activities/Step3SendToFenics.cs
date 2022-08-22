using System.Text.Json;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Dapr.AzureFunctions.Extension;
using FunctionDurable.LooseCup.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace FunctionDurable.LooseCup.Activities
{
    public static class Step3SendToFenicsActvity
    {
        [FunctionName("SendToFenics")]
        //[return: Queue("FenicsTaskQueue")]
        public static void SendToFenics(
            [ActivityTrigger] FenicsCommand command,
            [DaprPublish(PubSubName = "redis-pubsub", Topic = "FenicsTask")] out DaprPubSubEvent pubSubEvent,
            ILogger log)
        {
            log.LogWarning($"Send Trade {command.TradeId} to Fenics.");

            //var message = JsonSerializer.Serialize(command);
            var token = JToken.FromObject(command);
            pubSubEvent = new DaprPubSubEvent(token);

            //await Task.Delay(100);
            //return message;

        }

        [FunctionName("FenicsComplete")]
        public static async Task FenicsComplete(
            //[QueueTrigger("FenicsTaskCompleteQueue")] string fenicsCommand,
            [DaprTopicTrigger("redis-pubsub", Topic = "FenicsTaskComplete")] CloudEvent subEvent,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            //var result = JsonSerializer.Deserialize<FenicsCommand>(fenicsCommand);
            var result = JsonSerializer.Deserialize<FenicsCommand>(subEvent.Data.ToString());

            log.LogWarning($"Mission Fenics: {result.TradeId} has completed. FenicsId: {result.FenicsId} has returned.");

            await client.RaiseEventAsync(result.OrchestrationId, "FenicsTaskComplete", result.FenicsId);
        }

    }
}
