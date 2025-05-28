using Sample2.Models;

namespace Sample2
{
    public interface IDomainEvent
    {
        DateTime OccurredAt { get; }
    }

    public record OrderCreatedEvent(Order Order) : IDomainEvent
    {
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
    }
}
