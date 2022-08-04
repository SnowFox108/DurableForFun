using System;
using System.IO;
using System.Threading.Tasks;
using Dapr.Client;
using DaprFuncTester.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DaprFuncTester
{
    public static class ViewState
    {
        [FunctionName("ViewState")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string responseMessage = "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response.";

            string id = req.Query["id"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            id = id ?? data?.id;
            int key;

            if (int.TryParse(id, out key))
            {
                var client = new DaprClientBuilder().Build();
                var service = new DaprClientStateBasketService(client, log);
                var fruit =  await service.GetFruit(key);

                responseMessage = $"Get from Dapr state: {fruit.Name}";
            }

            return new OkObjectResult(responseMessage);
        }
    }
}
