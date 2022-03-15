using System;

namespace FunctionDurable.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsSaved { get; set; }
        public Order(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}
