using FunctionDurable.LooseCup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionDurable.Infrastructure
{
    public class TradeRepository
    {
        public TradeRepository()
        {

        }

        public Trade CreateTrade(string tradeName)
        {
            var trade = new Trade();
            trade.Name = tradeName;
            trade.Id = IdGenerator.Instance.NextId();

            return trade;
        }
    }
}
