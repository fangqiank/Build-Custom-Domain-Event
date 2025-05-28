namespace Sample2
{
    public class EmailNotificationHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly ILogger<EmailNotificationHandler> _logger;

        public EmailNotificationHandler(ILogger<EmailNotificationHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(OrderCreatedEvent @event)
        {
            _logger.LogInformation($"发送订单确认邮件给{@event.Order.CustomerName}，订单号：{@event.Order.Id}");
            return Task.CompletedTask;
        }
    }
}
