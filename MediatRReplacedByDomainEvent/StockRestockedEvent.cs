namespace MediatRReplacedByDomainEvent
{
    public record StockRestockedEvent(
    Guid ProductId,
    int Quantity) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
