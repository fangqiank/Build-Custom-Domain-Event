namespace BuildCustomDomainEvent
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }

        public OrderCreatedEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
