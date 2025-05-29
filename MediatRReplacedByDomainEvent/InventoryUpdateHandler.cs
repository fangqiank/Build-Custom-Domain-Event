using MediatRReplacedByDomainEvent.Data;

namespace MediatRReplacedByDomainEvent
{
    public class InventoryUpdateHandler(
        ILogger<InventoryUpdateHandler> logger,
        OrderDbContext context
        ) : IEventHandler<OrderCreatedEvent>
    {
        public async Task Handle(OrderCreatedEvent @event, CancellationToken ct)
        {
            // 模拟库存更新
            await Task.Delay(100, ct);
            logger.LogInformation("Inventory updated for order {OrderId}", @event.OrderId);
        }
    }
}
