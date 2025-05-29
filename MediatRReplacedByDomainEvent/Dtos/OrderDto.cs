namespace MediatRReplacedByDomainEvent.Dtos
{
    public record OrderDto(
    Guid Id,
    string CustomerName,
    decimal TotalAmount,
    DateTime CreatedAt,
    List<OrderItemDto> Items);

    public record OrderItemDto(
     Guid Id,
     string ProductName,
     int Quantity,
     decimal UnitPrice);
}
