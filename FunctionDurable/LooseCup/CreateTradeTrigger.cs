using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FunctionDurable.LooseCup
{
    public static class CreateTradeTrigger
    {
        [FunctionName(nameof(CreateTradeStarter))]
        public static async Task<IActionResult> CreateTradeStarter(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
        {
            var tradeName = req.GetQueryParameterDictionary()["name"];

            if (tradeName == null)
            {
                return new BadRequestObjectResult("Could not find any trade.");
            }

            string instanceId = await starter.StartNewAsync(nameof(CreateTradeWorkflow.CreateTradeOrchestrator), null, tradeName);
            log.LogWarning($"Started Trade orchestration with ID: {instanceId}.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

    }
}
