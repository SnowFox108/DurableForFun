using System;

namespace FunctionDurable.LooseCup.Models
{
    public class Trade
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Utilisation { get; set; }
        public DateTime Created { get; }

        public Trade()
        {
            Created = DateTime.Now;
        }
    }
}
