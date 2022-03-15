using System.Threading.Tasks;
using FunctionDurable.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionDurable.SimpleOrder.Activities
{
    public static class SaveOrderActivity
    {
        [FunctionName("SaveOrder")]
        public static bool SaveOrder([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"SaveOrder {name} successful.");
            return true;
        }
    }
}
