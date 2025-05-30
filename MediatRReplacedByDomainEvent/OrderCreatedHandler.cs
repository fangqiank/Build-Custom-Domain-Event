using Microsoft.AspNetCore.Identity.UI.Services;

namespace MediatRReplacedByDomainEvent
{
    public class OrderCreatedHandler(
        ILogger<OrderCreatedHandler> logger,
        IEmailService emailService
        ) : IEventHandler<OrderCreatedEvent>
    {
      
        public Task HandleAsync(OrderCreatedEvent @event, CancellationToken ct)
        {
            logger.LogInformation("Order {OrderId} created for {Email}", @event.OrderId, @event.CustomerEmail);
            return emailService.SendOrderConfirmation(@event.CustomerEmail, @event.OrderId);
        }
    }
}
