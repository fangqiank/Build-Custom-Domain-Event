namespace MediatRReplacedByDomainEvent
{
    public record OrderPaidEvent(
    Guid OrderId,
    decimal Amount) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}