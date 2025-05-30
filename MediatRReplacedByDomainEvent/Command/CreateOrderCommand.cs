using MediatRReplacedByDomainEvent;
using MediatRReplacedByDomainEvent.Command;
using MediatRReplacedByDomainEvent.Data;
using MediatRReplacedByDomainEvent.Dtos;
using MediatRReplacedByDomainEvent.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MediatRReplacedByDomainEvent.Command
{
    public record CreateOrderCommand(
    [Required, EmailAddress] string CustomerEmail,
    [MinLength(1)] List<OrderItemRequest> Items) : ICommand;
}

public class CreateOrderHandler(
    AppDbContext db,
    ILogger<CreateOrderHandler> logger
    ) : ICommandHandler<CreateOrderCommand>
{
   
    public async Task HandleAsync(CreateOrderCommand command, CancellationToken ct)
    {
        logger.LogInformation("Creating order for {CustomerEmail}", command.CustomerEmail);

        // 获取所有产品 ID
        var productIds = command.Items.Select(i => i.ProductId).ToList();

        logger.LogDebug("Requested product IDs: {ProductIds}", string.Join(", ", productIds));

        // 从数据库获取产品
        var products = await db.Products
            .Where(p => productIds.Contains(p.Id))
            .AsNoTracking()
            .ToDictionaryAsync(p => p.Id, ct);

        logger.LogDebug("Found {Count} products in database", products.Count);

        // 打印找到的产品ID以便调试
        var foundIds = products.Keys.Select(k => k.ToString()).ToList();
        logger.LogDebug("Found product IDs: {FoundProductIds}", string.Join(", ", foundIds));

        // 验证所有产品都存在
        var missingProducts = productIds.Except(products.Keys).ToList();
        if (missingProducts.Any())
        {
            var missingIds = string.Join(", ", missingProducts);
            logger.LogError("Products not found: {ProductIds}", missingIds);
            throw new ProductNotFoundException(missingProducts);
        }

        // 创建订单项
        var orderItems = new List<OrderItem>();
        foreach (var item in command.Items)
        {
            var product = products[item.ProductId];

            // 验证库存
            if (product.Stock < item.Quantity)
            {
                throw new InsufficientStockException(
                    product.Id,
                    product.Name,
                    product.Stock,
                    item.Quantity);
            }

            orderItems.Add(new OrderItem(
                product.Id,
                product.Name,
                product.Price,
                item.Quantity
            ));
        }

        // 创建订单
        var order = new Order(command.CustomerEmail, orderItems);
        db.Orders.Add(order);

        // 减少库存
        foreach (var item in orderItems)
        {
            products[item.ProductId].ReduceStock(item.Quantity);
        }

        await db.SaveChangesAsync(ct);
        logger.LogInformation("Order {OrderId} created successfully", order.Id);
    }

    public class ProductNotFoundException : Exception
    {
        public List<Guid> MissingProductIds { get; }

        public ProductNotFoundException(List<Guid> missingProductIds)
            : base($"Products not found: {string.Join(", ", missingProductIds)}")
        {
            MissingProductIds = missingProductIds;
        }
    }

    public class InsufficientStockException : Exception
    {
        public Guid ProductId { get; }
        public string ProductName { get; }
        public int AvailableStock { get; }
        public int RequestedQuantity { get; }

        public InsufficientStockException(Guid productId, string productName, int availableStock, int requestedQuantity)
            : base($"Insufficient stock for product {productName} ({productId}). " +
                   $"Available: {availableStock}, Requested: {requestedQuantity}")
        {
            ProductId = productId;
            ProductName = productName;
            AvailableStock = availableStock;
            RequestedQuantity = requestedQuantity;
        }
    }
}

