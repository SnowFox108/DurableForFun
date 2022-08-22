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
    [StorageAccount("AzureWebJobsStorage")]
    public static class Step2GeneratePdfActvity
    {
        [FunctionName("GenerateDocument")]
        //[return: Queue("PdfTaskQueue")]
        public static void GenerateDocument([ActivityTrigger] PdfCommand command,
            [DaprPublish(PubSubName = "redis-pubsub", Topic = "PdfTask")] out DaprPubSubEvent pubSubEvent,
                ILogger log)
        {
            log.LogWarning($"Sending request to Generating document for Trade {command.TradeId}.");

            //var message = JsonSerializer.Serialize(command);
            var token = JToken.FromObject(command);
            pubSubEvent = new DaprPubSubEvent(token);

            log.LogWarning($"message has been published to {pubSubEvent.PubSubName} : {pubSubEvent.Topic}");

            //await Task.Delay(100);
            //return message;
        }

        [FunctionName("GenerateDocumentComplete")]
        public static async Task GenerateDocumentComplete(
            //[QueueTrigger("PdfTaskCompleteQueue")] string pdfCommand,
            [DaprTopicTrigger("redis-pubsub", Topic = "PdfTaskComplete")] CloudEvent subEvent,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {

            //var result = JsonSerializer.Deserialize<PdfCommand>(pdfCommand);
            var result = JsonSerializer.Deserialize<PdfCommand>(subEvent.Data.ToString());

            log.LogWarning($"Mission PdfGenerator: {result.TradeId} has completed.");

            await client.RaiseEventAsync(result.OrchestrationId, "PdfTaskComplete", result.PdfPath);

        }

    }
}
