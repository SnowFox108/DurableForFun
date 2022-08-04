using System;
using System.IO;
using System.Threading.Tasks;
using Dapr.Client;
using DaprFuncTester.Models;
using DaprFuncTester.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DaprFuncTester
{
    public static class InputState
    {

        [FunctionName("InputState")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string responseMessage = "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response.";

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            if (!string.IsNullOrEmpty(name))
            {
                var client = new DaprClientBuilder().Build();
                var service = new DaprClientStateBasketService(client, log);

                await service.AddToBasket(new Fruit()
                {
                    Id = 1,
                    Name = name
                });

                responseMessage = $"Insert into Dapr state: {name}";
            }

            return new OkObjectResult(responseMessage);
        }
    }
}
