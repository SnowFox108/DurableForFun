using CloudNative.CloudEvents;
using Dapr.AzureFunctions.Extension;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace DaprFuncTester
{
    public static class SubBinding
    {
        [FunctionName("SubBinding")]
        public static void Run(
            [DaprTopicTrigger("redis-pubsub", Topic = "orders")] CloudEvent subEvent,
            ILogger log)
        {
            log.LogInformation("C# function processed a PrintTopicMessage request from the Dapr Runtime.");
            log.LogInformation($"Topic orders received a message: {subEvent.Data}.");
        }
    }
}