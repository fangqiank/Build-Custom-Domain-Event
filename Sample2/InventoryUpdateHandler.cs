namespace Sample2
{
    public class InventoryUpdateHandler : IAsyncEventHandler<OrderCreatedEvent>
    {
        private readonly ILogger<InventoryUpdateHandler> _logger;

        public InventoryUpdateHandler(ILogger<InventoryUpdateHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(OrderCreatedEvent @event)
        {
            foreach (var item in @event.Order.Items)
            {
                // 模拟库存更新操作
                await Task.Delay(100);
                _logger.LogInformation($"更新库存：产品 {item.ProductName} 减少 {item.Quantity} 件");
            }
        }
    }
}
