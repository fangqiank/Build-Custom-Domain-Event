namespace MediatRReplacedByDomainEvent
{
    public interface IEmailService
    {
        Task SendOrderConfirmation(string email, Guid orderId);
    }

    public class ConsoleEmailService : IEmailService
    {
        public Task SendOrderConfirmation(string email, Guid orderId)
        {
            Console.WriteLine($"Sending order confirmation to {email} for order {orderId}");
            return Task.CompletedTask;
        }
    }
}
