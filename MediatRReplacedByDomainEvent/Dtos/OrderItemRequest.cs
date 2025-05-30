using System.Text.Json.Serialization;

namespace MediatRReplacedByDomainEvent.Dtos
{
    public record OrderItemRequest(
    [property: JsonConverter(typeof(JsonStringGuidConverter))]
    Guid ProductId,
    int Quantity);
}
