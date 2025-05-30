namespace MediatRReplacedByDomainEvent
{
    public record OrderCreatedEvent(
    Guid OrderId,
    string CustomerEmail,
    decimal TotalAmount) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
