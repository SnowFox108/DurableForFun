using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;


namespace FunctionDurable
{
    public static class OrderSubmitTrigger
    {
        [FunctionName(nameof(OrderSubmitStarter))]
        public static async Task<IActionResult> OrderSubmitStarter(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
        {
            var orderName = req.GetQueryParameterDictionary()["name"];

            if (orderName == null)
            {
                return new BadRequestObjectResult("Could not find any order.");
            }

            string instanceId = await starter.StartNewAsync(nameof(CreateOrderWorkflow.CreateOrderOrchestrator), null, orderName);
            log.LogInformation($"Started orchestration with ID: {instanceId}.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
