using MediatRReplacedByDomainEvent.Data;

namespace MediatRReplacedByDomainEvent.Command
{
    public record CancelOrderCommand(Guid OrderId) : ICommand;

    public class CancelOrderHandler(AppDbContext db) : ICommandHandler<CancelOrderCommand>
    {
        public async Task HandleAsync(CancelOrderCommand command, CancellationToken ct)
        {
            var order = await db.Orders.FindAsync(command.OrderId);
            if (order == null) throw new Exception("Order not found");

            order.Cancel();
            await db.SaveChangesAsync(ct);
        }
    }
}
