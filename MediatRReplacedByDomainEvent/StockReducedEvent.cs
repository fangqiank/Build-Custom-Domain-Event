namespace MediatRReplacedByDomainEvent
{
    public record StockReducedEvent(
    Guid ProductId,
    int Quantity) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

}