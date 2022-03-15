using System.Text.Json;
using System.Threading.Tasks;
using FunctionDurable.LooseCup.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FunctionDurable.LooseCup.Activities
{
    [StorageAccount("AzureWebJobsStorage")]
    public static class Step2GeneratePdfActvity
    {
        [FunctionName("GenerateDocument")]
        [return: Queue("PdfTaskQueue")]
        public static async Task<string> GenerateDocument([ActivityTrigger] PdfCommand command, ILogger log)
        {
            log.LogInformation($"Generating document for Trade {command.TradeId}.");

            var message = JsonSerializer.Serialize(command);
           
            await Task.Delay(100);
            return message;
        }

        [FunctionName("GenerateDocumentComplete")]
        public static async Task GenerateDocumentComplete(
            [QueueTrigger("PdfTaskCompleteQueue")] string pdfCommand,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {

            var result = JsonSerializer.Deserialize<PdfCommand>(pdfCommand);

            log.LogInformation($"Mission PdfGenerator: {result.TradeId} has completed.");

            await client.RaiseEventAsync(result.OrchestrationId, "PdfTaskComplete", result.PdfPath);

        }

    }
}
