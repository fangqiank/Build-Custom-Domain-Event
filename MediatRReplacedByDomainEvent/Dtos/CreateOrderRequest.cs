namespace MediatRReplacedByDomainEvent.Dtos
{
    public record CreateOrderRequest(string CustomerName, List<OrderItemRequest> Items);
    public record OrderItemRequest(string ProductName, int Quantity, decimal UnitPrice);
}
