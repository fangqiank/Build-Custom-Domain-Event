using MediatRReplacedByDomainEvent.Data;

namespace MediatRReplacedByDomainEvent.Command
{
    public record PayOrderCommand(Guid OrderId) : ICommand;

    public class PayOrderHandler(AppDbContext db) : ICommandHandler<PayOrderCommand>
    {
        public async Task HandleAsync(PayOrderCommand command, CancellationToken ct)
        {
            var order = await db.Orders.FindAsync(command.OrderId);
            if (order == null) throw new Exception("Order not found");

            order.MarkAsPaid();
            await db.SaveChangesAsync(ct);
        }
    }
}
