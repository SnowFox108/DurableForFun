using System;
using System.IO;
using System.Threading.Tasks;
using Dapr.AzureFunctions.Extension;
using DaprFuncTester.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DaprFuncTester
{
    public static class PubBinding
    {
        [FunctionName("PubBinding")]
        public static void Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [DaprPublish(PubSubName = "redis-pubsub", Topic = "printer")] out DaprPubSubEvent pubSubEvent,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var basket = new Basket(1);
            var token = JToken.FromObject(basket);
            pubSubEvent = new DaprPubSubEvent(token);
            log.LogInformation($"message has been published to {pubSubEvent.PubSubName} : {pubSubEvent.Topic}");

        }
    }
}
