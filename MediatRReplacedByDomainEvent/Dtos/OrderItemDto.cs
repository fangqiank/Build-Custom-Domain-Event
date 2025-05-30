namespace MediatRReplacedByDomainEvent.Dtos
{
    public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice);
}
