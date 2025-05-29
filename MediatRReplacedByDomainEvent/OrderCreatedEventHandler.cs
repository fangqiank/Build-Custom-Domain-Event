namespace MediatRReplacedByDomainEvent
{
    public class OrderCreatedEventHandler(
        ILogger<OrderCreatedEventHandler> logger
        ) : IEventHandler<OrderCreatedEvent>
    {
        public Task Handle(OrderCreatedEvent @event, CancellationToken ct)
        {
            logger.LogInformation(
                "Sending email confirmation for order {OrderId} to {Customer}",
                @event.OrderId, @event.CustomerName);

            return Task.CompletedTask;
        }
    }
}
