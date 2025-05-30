using MediatRReplacedByDomainEvent.Data;
using MediatRReplacedByDomainEvent.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MediatRReplacedByDomainEvent.Query
{
    public record GetAllProductsQuery : IQuery<IEnumerable<ProductDto>>;

    public class GetAllProductsHandler(AppDbContext db) : IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
    {
        public async Task<IEnumerable<ProductDto>> HandleAsync(GetAllProductsQuery query, CancellationToken ct)
        {
            return await db.Products
                .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock))
                .ToListAsync(ct);
        }
    }
}
