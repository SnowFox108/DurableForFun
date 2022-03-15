using Microsoft.WindowsAzure.Storage.Table;

namespace PdfGenerator
{
    public class PdfTaskEntity // : TableEntity
    {
        public string OrchestrationId { get; set; }
        public int TradeId { get; set; }
        public bool IsProcessed { get; set; }
    }
}
