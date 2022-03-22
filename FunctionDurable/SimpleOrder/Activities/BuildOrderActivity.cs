using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FunctionDurable.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionDurable.Activities
{
    public static class BuildOrderActivity
    {
        [FunctionName("BuildOrder")]
        public static Order BuildOrder([ActivityTrigger] string name, ILogger log)
        {
            log.LogWarning($"BuildOrder {name}.");
            return new Order(name);
        }
    }
}