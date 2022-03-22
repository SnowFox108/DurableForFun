using System.Threading.Tasks;
using FunctionDurable.LooseCup.Activities;
using FunctionDurable.LooseCup.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FunctionDurable.LooseCup.Workflows
{
    public static class CreateTradeStep1Workflow
    {

        [FunctionName(nameof(CreateTradeStep1Orchestrator))]
        public static async Task<Trade> CreateTradeStep1Orchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var tradeName = context.GetInput<string>();

            log.LogWarning($"Starting the Step1SaveToDatabase Activity {context.InstanceId} for Trade {tradeName}");
            var trade = await context.CallActivityAsync<Trade>(nameof(Step1SaveToDatabaseActivity.SaveToDatabase), tradeName);

            log.LogWarning($"Starting the Step1Utilisation Activity {context.InstanceId} for Trade {tradeName}");
            trade.Utilisation = await context.CallActivityAsync<decimal>(nameof(Step1UtilisationActivity.Utilisation), trade.Id);
            
            return trade;
        }
    }

}
