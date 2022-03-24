using Microsoft.WindowsAzure.Storage.Table;

namespace FenicsDispatcher.Infrastructure
{
    public class IdGeneratorEntity : TableEntity
    {
        public int CounterId { get; set; }

    }
}
