namespace MediatRReplacedByDomainEvent
{
    public class OrderCancelledHandler(ILogger<OrderCancelledHandler> logger) : IEventHandler<OrderCancelledEvent>
    {
        public Task HandleAsync(OrderCancelledEvent @event, CancellationToken ct)
        {
            logger.LogInformation("Order {OrderId} cancelled", @event.OrderId);
            return Task.CompletedTask;
        }
    }
}
