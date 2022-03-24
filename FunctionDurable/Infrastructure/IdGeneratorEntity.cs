using Microsoft.WindowsAzure.Storage.Table;

namespace FunctionDurable.Infrastructure
{
    public class IdGeneratorEntity : TableEntity
    {
        public int CounterId { get; set; }

    }
}
