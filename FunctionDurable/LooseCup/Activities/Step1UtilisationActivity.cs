using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace FunctionDurable.LooseCup.Activities
{
    public static class Step1UtilisationActivity
    {
        [FunctionName("Utilisation")]
        public static async Task<decimal> Utilisation([ActivityTrigger] int id, ILogger log)
        {
            log.LogWarning($"Caculate utilistion for Trade {id} at local Activity.");
            var rng = new Random();
            var result = rng.NextDecimal();
            
            await Task.Delay(100);
            log.LogWarning($"utilistion for Trade {id} was {result}.");

            return result;
        }

        private static decimal NextDecimal(this Random rng)
        {
            byte scale = (byte)rng.Next(29);
            bool sign = false; // rng.Next(2) == 1;
            return new decimal(rng.NextInt32(),
                               rng.NextInt32(),
                               rng.NextInt32(),
                               sign,
                               scale);
        }

        private static int NextInt32(this Random rng)
        {
            int firstBits = rng.Next(0, 1 << 4) << 28;
            int lastBits = rng.Next(0, 1 << 28);
            return firstBits | lastBits;
        }

    }
}
