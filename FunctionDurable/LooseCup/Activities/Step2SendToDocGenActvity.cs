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
    public static class Step2SendToDocGenActvity
    {
        [FunctionName("SendToDocGen")]
        //[return: Queue("DocGenTaskQueue")]
        public static void SendToDocGen(
            [ActivityTrigger] DocGenCommand command,
            [DaprPublish(PubSubName = "redis-pubsub", Topic = "DocGenTask")] out DaprPubSubEvent pubSubEvent,
            ILogger log)
        {
            log.LogWarning($"Send email message to DocGen for Trade {command.TradeId}.");

            //var message = JsonSerializer.Serialize(command);
            var token = JToken.FromObject(command);
            pubSubEvent = new DaprPubSubEvent(token);

            //await Task.Delay(100);
            //return message;
        }

        [FunctionName("DocGenComplete")]
        public static async Task GenerateDocumentComplete(
            //[QueueTrigger("DocGenTaskCompleteQueue")] string docGenCommand,
            [DaprTopicTrigger("redis-pubsub", Topic = "DocGenTaskComplete")] CloudEvent subEvent,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            //var result = JsonSerializer.Deserialize<DocGenCommand>(docGenCommand);
            var result = JsonSerializer.Deserialize<DocGenCommand>(subEvent.Data.ToString());

            log.LogWarning($"Mission DocGen dispatch: {result.TradeId} has completed.");

            await client.RaiseEventAsync(result.OrchestrationId, "DocGenTaskComplete");
        }

    }
}
