namespace FunctionDurable.LooseCup.Models
{
    public class PdfCommand
    {
        public int TradeId { get; set; }
        public string OrchestrationId { get; set; }
        public string PdfPath { get; set; }
    }
}
