using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MigraDoc.Rendering;
using PdfSharp.Pdf;

namespace PdfGenerator
{
    [StorageAccount("AzureWebJobsStorage")]
    public class DocumentProcessor
    {
        [FunctionName("DocumentProcessor")]
        [return: Queue("PdfTaskCompleteQueue")]
        public async Task<string> Run([TimerTrigger("*/1 * * * *")]TimerInfo myTimer,
            //[Table("PdfTask", "", Filter = "IsProcessed eq 'false'")] IEnumerable<PdfTaskEntity>,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var result = await TaskProcessor(log);
            return result;
        }

        [FunctionName("DocumentBuilder")]
        [return: Queue("PdfTaskCompleteQueue")]
        public async Task<string> Builder(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,

            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var result = await TaskProcessor(log);
            return result;
        }

        private async Task<string> TaskProcessor(ILogger log)
        {
            var condition = TableQuery.GenerateFilterConditionForBool("IsProcessed", QueryComparisons.Equal, false);

            var query = new TableQuery<PdfTaskEntity>().Where(condition);

            var account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process));
            var client = account.CreateCloudTableClient();

            var table = client.GetTableReference("PdfTask");
            await table.CreateIfNotExistsAsync();

            var lst = await table.ExecuteQuerySegmentedAsync(query, null);
            var task = lst.FirstOrDefault();

            if (task == null)
            {
                log.LogInformation($"There is no Pdf document waiting in the queue.");

                return null;
            }
            else
            { 
                // disable PdfSharp
                //await SaveToBlob();


                // other database update
                log.LogInformation($"Build Pdf document for {task.TradeId}");

                task.IsProcessed = true;
                var command = new PdfCommand()
                {
                    TradeId = task.TradeId,
                    OrchestrationId = task.OrchestrationId,
                    PdfPath = $"{task.TradeId}.pdf"
                };

                var message = JsonSerializer.Serialize(command);

                TableOperation updateOperation = TableOperation.Replace(task);
                await table.ExecuteAsync(updateOperation);

                return message;
            }
        }

        private async Task SaveToBlob()
        {
            var pdfBuilder = new PdfBuilder();
            var document = pdfBuilder.BuildDocument();

            // ===== Unicode encoding and font program embedding in MigraDoc is demonstrated here =====

            // A flag indicating whether to create a Unicode PDF or a WinAnsi PDF file.
            // This setting applies to all fonts used in the PDF document.
            // This setting has no effect on the RTF renderer.
            const bool unicode = true;

            // An enum indicating whether to embed fonts or not.
            // This setting applies to all font programs used in the document.
            // This setting has no effect on the RTF renderer.
            // (The term 'font program' is used by Adobe for a file containing a font. Technically a 'font file'
            // is a collection of small programs and each program renders the glyph of a character when executed.
            // Using a font in PDFsharp may lead to the embedding of one or more font programms, because each outline
            // (regular, bold, italic, bold+italic, ...) has its own fontprogram)
            const PdfFontEmbedding embedding = PdfFontEmbedding.Always;

            // ========================================================================================

            // Create a renderer for the MigraDoc document.
            var pdfRenderer = new PdfDocumentRenderer(unicode);

            // Associate the MigraDoc document with a renderer
            pdfRenderer.Document = document;
            //const string filename = @"..\..\..\..\SupportFiles\HelloWorld.pdf";

            // Layout and render document to PDF
            pdfRenderer.RenderDocument();

            // Save the document...
            //pdfRenderer.PdfDocument.Save(filename);
            // ...and start a viewer.
            //Process.Start(filename);


            var containerName = "foxoption-contract-notes";
            //// Create a BlobServiceClient object which will be used to create a container client
            //BlobServiceClient blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            //// Create the container and return a container client object
            //BlobContainerClient containerClient = blobServiceClient.CreateBlobContainer(containerName);


            var localPath = "./data/";
            var fileName = $"Contract{Guid.NewGuid().ToString()}.pdf";
            var localFilePath = Path.Combine(localPath, fileName);


            var cloudStorageAccount =
                CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process));
            //create a block blob CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();  
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            //create a container CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("appcontainer");

            var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            await cloudBlobContainer.CreateIfNotExistsAsync();


            var ms = new MemoryStream();
            pdfRenderer.PdfDocument.Save(ms);

            //get Blob reference  
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(localFilePath);
            await cloudBlockBlob.UploadFromStreamAsync(ms);

        }
    }
}
