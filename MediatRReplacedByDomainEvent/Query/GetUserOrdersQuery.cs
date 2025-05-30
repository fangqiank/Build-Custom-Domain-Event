using MediatRReplacedByDomainEvent.Data;
using MediatRReplacedByDomainEvent.Dtos;
using MediatRReplacedByDomainEvent.Models;
using Microsoft.EntityFrameworkCore;

namespace MediatRReplacedByDomainEvent.Query
{
    public record GetUserOrdersQuery(string Email) : IQuery<IEnumerable<OrderDto>>;

    public class GetUserOrdersHandler(AppDbContext db) : IQueryHandler<GetUserOrdersQuery, IEnumerable<OrderDto>>
    {
        public async Task<IEnumerable<OrderDto>> HandleAsync(GetUserOrdersQuery query, CancellationToken ct)
        {
            var orders = await db.Orders
            .Where(o => o.CustomerEmail == query.Email)
            .Select(o => new OrderDto(
                o.Id,
                o.CustomerEmail,
                o.CreatedAt,
                o.Status,
                o.Items.Sum(i => i.UnitPrice * i.Quantity),
                o.Items.Select(i => new OrderItemDto(
                    i.ProductId,
                    i.ProductName,
                    i.UnitPrice,
                    i.Quantity,
                    i.UnitPrice * i.Quantity
                ))
            ))
            .AsNoTracking()
            .ToListAsync(ct);

            return orders;
        }
    }
}
