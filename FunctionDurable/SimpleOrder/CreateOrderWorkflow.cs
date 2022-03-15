using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FunctionDurable.Activities;
using FunctionDurable.Models;
using FunctionDurable.SimpleOrder.Activities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FunctionDurable
{
    public static class CreateOrderWorkflow
    {
        [FunctionName(nameof(CreateOrderOrchestrator))]
        public static async Task<object> CreateOrderOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log
            )
        {
            log = context.CreateReplaySafeLogger(log);
            var orderName = context.GetInput<string>();

            var order = await context.CallActivityAsync<Order>(nameof(BuildOrderActivity.BuildOrder), orderName);

            order.IsSaved = await context.CallActivityAsync<bool>(nameof(SaveOrderActivity.SaveOrder), orderName);

            return order;
        }
    }
}
