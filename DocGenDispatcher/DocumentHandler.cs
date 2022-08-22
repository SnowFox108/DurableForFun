using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Dapr.AzureFunctions.Extension;
using DocGenDispatcher;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace PdfGenerator
{
    [StorageAccount("AzureWebJobsStorage")]
    public class DocumentHandler
    {
        [FunctionName("DocumentHandler")]
        //[return: Queue("DocGenTaskCompleteQueue")]
        public static void Run(
            //[QueueTrigger("DocGenTaskQueue")] string docGenCommand,
            [DaprTopicTrigger("redis-pubsub", Topic = "DocGenTask")] CloudEvent subEvent,
            [DaprPublish(PubSubName = "redis-pubsub", Topic = "DocGenTaskComplete")] out DaprPubSubEvent pubSubEvent,
            ILogger log)
        {
            //var command = JsonSerializer.Deserialize<DocGenCommand>(docGenCommand);
            var command = JsonSerializer.Deserialize<DocGenCommand>(subEvent.Data.ToString());

            log.LogInformation($"New DocGen task has been put in: Task {command.TradeId}");

            //await Task.Delay(1000);
            Thread.Sleep(100);
            //var message = JsonSerializer.Serialize(command);
            var token = JToken.FromObject(command);
            pubSubEvent = new DaprPubSubEvent(token);

            //return message;

        }
    }
}
