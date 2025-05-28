namespace Sample2.Dtos
{
    public record CreateOrderRequest(
     string CustomerName, // 属性名必须与 JSON 字段匹配
     List<OrderItemRequest> Items
 );

    public record OrderItemRequest(
        string ProductName,
        int Quantity,
        decimal UnitPrice
    );
}
