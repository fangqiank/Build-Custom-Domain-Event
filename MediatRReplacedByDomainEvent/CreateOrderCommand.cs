using MediatRReplacedByDomainEvent.Dtos;

namespace MediatRReplacedByDomainEvent
{
    public record CreateOrderCommand(
    string CustomerName,
    List<OrderItemRequest> Items
) : ICommand<CreateOrderResult>;

    // 命令结果
    public record CreateOrderResult(Guid OrderId);

    // 领域事件
    public record OrderCreatedEvent(Guid OrderId, string CustomerName) : IDomainEvent;
}
