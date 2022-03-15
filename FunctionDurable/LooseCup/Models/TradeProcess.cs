using System;

namespace FunctionDurable.LooseCup.Models
{
    public class TradeProcess
    {
        public int TradeId { get; set; }
        public string TradeName { get; set;}
        public DateTime TradeCreated { get; set; }

        public bool IsSaveToDatabase { get; set; }
        public decimal Utilisation { get; set; }

        public string PdfPath { get; set; }
        public bool IsPdfGenerated { get; set; }
        public bool IsDocGenDispatched { get; set; }

        public int FenicId { get; set; }
        public bool IsFenicCreated { get; set; }

    }
}
