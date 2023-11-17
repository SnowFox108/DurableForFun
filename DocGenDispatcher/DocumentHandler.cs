using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Dapr.AzureFunctions.Extension;
using DocGenDispatcher;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PdfGenerator.Infrastructure;

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

        [FunctionName("TestDatabaseRead")]
        public async Task<IActionResult> WebRun(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            //Environment.SetEnvironmentVariable("DATABASE_STRING", "C3015;Initial Catalog=Options;Integrated Security=True;MultipleActiveResultSets=true;");
            log.LogInformation($"Http based call for TestDatabaseRead");

            var processor = new DatabaseProcessor();

            await processor.ProcessTask(log);

            log.LogInformation($"Http based call for TestDatabaseRead is completed.");

            return new OkObjectResult($"TestDatabaseRead with Ok Result");

        }
    }
}
