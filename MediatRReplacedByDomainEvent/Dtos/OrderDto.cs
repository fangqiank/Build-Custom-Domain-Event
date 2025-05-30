using MediatRReplacedByDomainEvent.Models;

namespace MediatRReplacedByDomainEvent.Dtos
{
    public record OrderDto(
    Guid Id,
    string CustomerEmail,
    DateTime CreatedAt,
    OrderStatus Status,
    decimal TotalAmount,
    IEnumerable<OrderItemDto> Items);
}
