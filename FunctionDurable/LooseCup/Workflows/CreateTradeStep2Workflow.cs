using System;
using System.Threading.Tasks;
using FunctionDurable.LooseCup.Activities;
using FunctionDurable.LooseCup.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FunctionDurable.LooseCup.Workflows
{
    public static class CreateTradeStep2Workflow
    {

        [FunctionName(nameof(CreateTradeStep2Orchestrator))]
        public static async Task<TradeProcess> CreateTradeStep2Orchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var tradeProcess = context.GetInput<TradeProcess>();

            log.LogWarning($"Starting the Step2GeneratePdf Activity {context.InstanceId} for Trade {tradeProcess.TradeId}");
            var pdfCcommand = new PdfCommand()
            {
                TradeId = tradeProcess.TradeId,
                OrchestrationId = context.InstanceId
            };

            await context.CallActivityAsync(nameof(Step2GeneratePdfActvity.GenerateDocument), pdfCcommand);

            try
            {
                tradeProcess.PdfPath = await context.WaitForExternalEvent<string>("PdfTaskComplete", TimeSpan.FromSeconds(300));
                tradeProcess.IsPdfGenerated = true;
            }
            catch (TimeoutException)
            {
                log.LogError($"TradeId: {tradeProcess.TradeId}, Timed out waiting for Pdf Generator.");
            }

            if (!string.IsNullOrEmpty(tradeProcess.PdfPath))
            {
                log.LogWarning($"Starting the Step2SendToDocGen Activity {context.InstanceId} for Trade {tradeProcess.TradeId}");
                var docGenCommand = new DocGenCommand()
                {
                    TradeId = tradeProcess.TradeId,
                    OrchestrationId = context.InstanceId
                };
                await context.CallActivityAsync(nameof(Step2SendToDocGenActvity.SendToDocGen), docGenCommand);

                try
                {
                    await context.WaitForExternalEvent<string>("DocGenTaskComplete", TimeSpan.FromSeconds(300));
                    tradeProcess.IsDocGenDispatched = true;
                }
                catch (TimeoutException)
                {
                    log.LogWarning($"TradeId: {tradeProcess.TradeId}, Timed out waiting for DocGen Dispatcher.");
                }

            }

            return tradeProcess;

        }
    }

}
