namespace MediatRReplacedByDomainEvent
{
    public class OrderPaidHandler(
        ILogger<OrderPaidHandler> logger,
        IShippingService shippingService

        ) : IEventHandler<OrderPaidEvent>
    {
        public Task HandleAsync(OrderPaidEvent @event, CancellationToken ct)
        {
            logger.LogInformation("Order {OrderId} paid, amount: {Amount}", @event.OrderId, @event.Amount);
            return shippingService.ScheduleShipping(@event.OrderId);
        }
    }
}
