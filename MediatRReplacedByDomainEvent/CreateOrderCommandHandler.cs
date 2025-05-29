using MediatRReplacedByDomainEvent.Data;
using MediatRReplacedByDomainEvent.Models;

namespace MediatRReplacedByDomainEvent
{
    public class CreateOrderCommandHandler(
        OrderDbContext context,
        IEventDispatcher eventDispatcher,
        ILogger<CreateOrderCommandHandler> logger
        ) : ICommandHandler<CreateOrderCommand, CreateOrderResult>
    {
        public async Task<CreateOrderResult> Handle(
            CreateOrderCommand command,
            CancellationToken ct = default)
        {
            // 创建订单
            var order = new Order { CustomerName = command.CustomerName };

            foreach (var item in command.Items)
            {
                order.AddItem(item.ProductName, item.Quantity, item.UnitPrice);
            }

            // 保存到数据库
            context.Orders.Add(order);
            await context.SaveChangesAsync(ct);

            logger.LogInformation("Created order {OrderId}", order.Id);

            // 发布领域事件
            await eventDispatcher.DispatchAsync(new OrderCreatedEvent(order.Id, order.CustomerName), ct);

            return new CreateOrderResult(order.Id);
        }
    }
}
