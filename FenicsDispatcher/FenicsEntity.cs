using Microsoft.WindowsAzure.Storage.Table;

namespace FenicsDispatcher
{
    public class FenicsTaskEntity : TableEntity
    {
        public string OrchestrationId { get; set; }
        public int TradeId { get; set; }
        public int FenicsId { get; set; }
        public bool IsProcessed { get; set; }


    }
}
