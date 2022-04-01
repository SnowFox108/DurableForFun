using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FenicsDispatcher.Infrastructure
{
    public class DataContext
    {
        public DataContext()
        {
        }

        public async Task<int> NextId()
        {
            var account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
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

            var condition = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, "FenicsGeneratorKey");
            var query = new TableQuery<IdGeneratorEntity>().Where(condition);
            var lst = await table.ExecuteQuerySegmentedAsync(query, null);

            var key = lst.FirstOrDefault();
            key.CounterId++;

            TableOperation updateOperation = TableOperation.Replace(key);
            await table.ExecuteAsync(updateOperation);

            return key.CounterId;
        }

    }
}
