using System.ComponentModel.DataAnnotations;

namespace MediatRReplacedByDomainEvent.Models
{
    public class Order : Entity<Guid>
    {
        public string CustomerEmail { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private Order() { } // For EF Core

        public Order(string customerEmail, List<OrderItem> items)
        {
            if (string.IsNullOrWhiteSpace(customerEmail))
                throw new ArgumentException("Customer email is required");

            if (!items.Any())
                throw new ArgumentException("Order must have at least one item");

            CustomerEmail = customerEmail;
            _items = items;

            // 手动计算总金额
            var totalAmount = items.Sum(i => i.UnitPrice * i.Quantity);
            RaiseEvent(new OrderCreatedEvent(Id, customerEmail, totalAmount));
        }

        public void MarkAsPaid()
        {
            if (Status != OrderStatus.Pending)
                throw new Exception("Only pending orders can be paid");

            // 手动计算总金额
            var totalAmount = Items.Sum(i => i.UnitPrice * i.Quantity);
            Status = OrderStatus.Paid;
            RaiseEvent(new OrderPaidEvent(Id, totalAmount));
        }


        public void Cancel()
        {
            if (Status != OrderStatus.Pending)
                throw new Exception("Only pending orders can be cancelled");

            Status = OrderStatus.Cancelled;
            RaiseEvent(new OrderCancelledEvent(Id));
        }
    }
}
