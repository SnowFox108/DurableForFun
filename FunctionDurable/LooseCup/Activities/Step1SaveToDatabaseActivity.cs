using System.Threading.Tasks;
using FunctionDurable.Infrastructure;
using FunctionDurable.LooseCup.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FunctionDurable.LooseCup.Activities
{
    public static class Step1SaveToDatabaseActivity
    {

        [FunctionName("SaveToDatabase")]
        public static async Task<Trade> SaveToDatabase([ActivityTrigger] string name, ILogger log)
        {
            log.LogWarning($"Saving Trade {name} at local Activity.");

            var _tradeRepository = new TradeRepository();
            var trade = await _tradeRepository.CreateTrade(name);

            log.LogWarning($"Trade {trade.Id} was created.");

            return trade;
        }


    }
}
