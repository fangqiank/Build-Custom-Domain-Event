namespace MediatRReplacedByDomainEvent
{
    public interface IShippingService
    {
        Task ScheduleShipping(Guid orderId);
    }

    public class MockShippingService : IShippingService
    {
        public Task ScheduleShipping(Guid orderId)
        {
            Console.WriteLine($"Scheduling shipping for order {orderId}");
            return Task.CompletedTask;
        }
    }
}
