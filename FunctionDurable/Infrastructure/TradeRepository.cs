using FunctionDurable.LooseCup.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
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

        public async Task<Trade> CreateTrade(string tradeName)
        {
            var account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process));
            var client = account.CreateCloudTableClient();

            var table = client.GetTableReference("IdGenerator");

            var isFirstTime = await table.CreateIfNotExistsAsync();

            if (isFirstTime)
            {
                // Insert Data for Fist time only
                var tradeKey = new IdGeneratorEntity
                {
                    PartitionKey = "IdGenerator",
                    RowKey = "TradeGeneratorKey",
                    CounterId = 2000
                };

                var fenicsKey = new IdGeneratorEntity
                {
                    PartitionKey = "IdGenerator",
                    RowKey = "FenicsGeneratorKey",
                    CounterId = 5000
                };

                TableOperation insertOperation = TableOperation.Insert(tradeKey);
                await table.ExecuteAsync(insertOperation);
                insertOperation = TableOperation.Insert(fenicsKey);
                await table.ExecuteAsync(insertOperation);
            }

            var condition = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, "TradeGeneratorKey");
            var query = new TableQuery<IdGeneratorEntity>().Where(condition);
            var lst = await table.ExecuteQuerySegmentedAsync(query, null);

            var key = lst.FirstOrDefault();
            key.CounterId++;

            TableOperation updateOperation = TableOperation.Replace(key);
            await table.ExecuteAsync(updateOperation);

            var trade = new Trade();
            trade.Name = tradeName;
            trade.Id = key.CounterId;

            return trade;
        }
    }
}
