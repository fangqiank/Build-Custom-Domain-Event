using System.ComponentModel.DataAnnotations;

namespace MediatRReplacedByDomainEvent.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<OrderItem> Items { get; set; } = new();

        public void AddItem(string productName, int quantity, decimal unitPrice)
        {
            Items.Add(new OrderItem
            {
                ProductName = productName,
                Quantity = quantity,
                UnitPrice = unitPrice
            });
            TotalAmount = Items.Sum(i => i.Quantity * i.UnitPrice);
        }
    }

    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
