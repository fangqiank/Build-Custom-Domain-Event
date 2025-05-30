namespace MediatRReplacedByDomainEvent.Dtos
{
    public record ProductDto(
    Guid Id,
    string Name,
    decimal Price,
    int Stock);
}
