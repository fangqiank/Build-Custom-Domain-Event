using MediatRReplacedByDomainEvent.Dtos;

namespace MediatRReplacedByDomainEvent
{
    public class OrderService(
        ICommandDispatcher commandDispatcher,
        ILogger<OrderService> logger
        )
    {
        public async Task<CreateOrderResult> CreateOrder(CreateOrderRequest request)
        {
            logger.LogInformation("Processing order for {Customer}", request.CustomerName);

            // 创建命令
            var command = new CreateOrderCommand(
                request.CustomerName,
                request.Items);

            // 通过命令分发器执行命令（会触发管道行为）
            return await commandDispatcher.Dispatch<CreateOrderCommand, CreateOrderResult>(command);
        }
    }
}
