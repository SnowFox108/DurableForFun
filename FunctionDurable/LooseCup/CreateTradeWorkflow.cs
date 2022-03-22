using System;
using System.Threading.Tasks;
using FunctionDurable.LooseCup.Activities;
using FunctionDurable.LooseCup.Models;
using FunctionDurable.LooseCup.Workflows;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FunctionDurable.LooseCup
{
    public static class CreateTradeWorkflow
    {
        [FunctionName(nameof(CreateTradeOrchestrator))]
        public static async Task<object> CreateTradeOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log
            )
        {
            log = context.CreateReplaySafeLogger(log);
            var tradeName = context.GetInput<string>();
            var tradeProcess = new TradeProcess();
            tradeProcess.TradeName = tradeName;

            log.LogInformation("about to call Step1 Orchestrator");
            var trade = await context.CallSubOrchestratorAsync<Trade>(nameof(CreateTradeStep1Workflow.CreateTradeStep1Orchestrator), tradeName);
            tradeProcess.TradeId = trade.Id;
            tradeProcess.Utilisation = trade.Utilisation;
            tradeProcess.TradeCreated = trade.Created;
            tradeProcess.IsSaveToDatabase = true;

            log.LogInformation("about to call Step2 Orchestrator");
            var step2Result = await context.CallSubOrchestratorAsync<TradeProcess>(nameof(CreateTradeStep2Workflow.CreateTradeStep2Orchestrator), tradeProcess);
            tradeProcess.PdfPath = step2Result.PdfPath;
            tradeProcess.IsPdfGenerated = step2Result.IsPdfGenerated;
            tradeProcess.IsDocGenDispatched = step2Result.IsDocGenDispatched;

            log.LogInformation("about to call Step3 Activity");
            var fenicsCommand = new FenicsCommand()
            {
                TradeId = tradeProcess.TradeId,
                OrchestrationId = context.InstanceId
            };

            await context.CallActivityAsync<int>(nameof(Step3SendToFenicsActvity.SendToFenics), fenicsCommand);

            try
            {
                tradeProcess.FenicId = await context.WaitForExternalEvent<int>("FenicsTaskComplete", TimeSpan.FromSeconds(300));
                tradeProcess.IsFenicCreated = true;

                log.LogInformation($"All Trade Id: {tradeProcess.TradeId} has completed.");
            }
            catch (TimeoutException)
            {
                log.LogWarning($"TradeId: {tradeProcess.TradeId}, Timed out waiting for Fenics.");
            }

            return tradeProcess;
        }

    }
}
