using System.ComponentModel.DataAnnotations.Schema;

namespace MediatRReplacedByDomainEvent.Models
{
    public class OrderItem
    {
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }

        [NotMapped] // 明确标记为不映射
        public decimal TotalPrice => UnitPrice * Quantity;

        private OrderItem() { } // EF Core 需要

        public OrderItem(Guid productId, string productName, decimal unitPrice, int quantity)
        {
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}
