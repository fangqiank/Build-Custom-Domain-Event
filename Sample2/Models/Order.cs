using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample2.Models
{
    public abstract class EntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; protected set; }

        private readonly List<IDomainEvent> _domainEvents = new();

        [NotMapped] // EF Core 忽略此属性
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }

    // ======================== 实体实现 ========================
    public class Order : EntityBase
    {
        public string CustomerName { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public List<OrderItem> Items { get; set; } = new();

        public void MarkAsCreated()
        {
            AddDomainEvent(new OrderCreatedEvent(this));
        }
    }

    public class OrderItem : EntityBase
    {
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
