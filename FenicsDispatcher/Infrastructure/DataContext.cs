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
            var account = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process));
            var client = account.CreateCloudTableClient();

            var table = client.GetTableReference("IdGenerator");

            await table.CreateIfNotExistsAsync();

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
