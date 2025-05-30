namespace MediatRReplacedByDomainEvent
{
    public class StockReducedHandler(
        ILogger<StockReducedHandler> logger,
        IInventoryService inventoryService
        ) : IEventHandler<StockReducedEvent>
    {
        public Task HandleAsync(StockReducedEvent @event, CancellationToken ct)
        {
            logger.LogInformation("Stock reduced for product {ProductId}, quantity: {Quantity}",
                @event.ProductId, @event.Quantity);

            return inventoryService.UpdateInventory(@event.ProductId, -@event.Quantity);
        }
    }
}
