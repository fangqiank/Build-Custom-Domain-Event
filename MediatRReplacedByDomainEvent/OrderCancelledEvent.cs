namespace MediatRReplacedByDomainEvent
{
    public record OrderCancelledEvent(Guid OrderId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}