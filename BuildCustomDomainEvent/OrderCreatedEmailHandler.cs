using BuildCustomDomainEvent.Models;

namespace BuildCustomDomainEvent
{
    public class OrderCreatedEmailHandler : IDomainEventHandler<OrderCreatedEvent>
    {
        public async Task Handle(OrderCreatedEvent @event)
        {
            Console.WriteLine($"[Event] Email sent for order: {@event.OrderId}");
            await Task.CompletedTask;
        }
    }
}
