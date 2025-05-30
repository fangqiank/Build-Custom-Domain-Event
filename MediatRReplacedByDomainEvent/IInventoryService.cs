namespace MediatRReplacedByDomainEvent
{
    public interface IInventoryService
    {
        Task UpdateInventory(Guid productId, int delta);
    }

    public class MockInventoryService : IInventoryService
    {
        public Task UpdateInventory(Guid productId, int delta)
        {
            Console.WriteLine($"Updating inventory for product {productId}: {(delta > 0 ? "+" : "")}{delta}");
            return Task.CompletedTask;
        }
    }
}
