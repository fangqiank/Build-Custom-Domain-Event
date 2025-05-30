using MediatRReplacedByDomainEvent.Data;

namespace MediatRReplacedByDomainEvent.Command
{
    public record RestockProductCommand(Guid ProductId, int Quantity) : ICommand;

    public class RestockProductHandler(AppDbContext db) : ICommandHandler<RestockProductCommand>
    {
        public async Task HandleAsync(RestockProductCommand command, CancellationToken ct)
        {
            var product = await db.Products.FindAsync(command.ProductId);
            if (product == null) throw new Exception("Product not found");

            product.Restock(command.Quantity);
            await db.SaveChangesAsync(ct);
        }
    }
}
