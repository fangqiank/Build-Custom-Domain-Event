using MediatRReplacedByDomainEvent.Data;
using MediatRReplacedByDomainEvent.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MediatRReplacedByDomainEvent.Query
{
    public record GetOrderByIdQuery(Guid OrderId) : IQuery<OrderDto>;

    public class GetOrderByIdHandler(AppDbContext db) : IQueryHandler<GetOrderByIdQuery, OrderDto>
    {
        public async Task<OrderDto> HandleAsync(GetOrderByIdQuery query, CancellationToken ct)
        {
            var result = await db.Orders
            .Where(o => o.Id == query.OrderId)
            .Select(o => new
            {
                Order = o,
                Items = o.Items.Select(i => new OrderItemDto(
                    i.ProductId,
                    i.ProductName,
                    i.UnitPrice,
                    i.Quantity,
                    i.UnitPrice * i.Quantity
                )).ToList(),
                TotalAmount = o.Items.Sum(i => i.UnitPrice * i.Quantity)
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

            if (result == null)
                return null;

            return new OrderDto(
                result.Order.Id,
                result.Order.CustomerEmail,
                result.Order.CreatedAt,
                result.Order.Status,
                result.TotalAmount,
                result.Items
            );
        }
    }

}
