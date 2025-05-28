using BuildCustomDomainEvent.Data;
using BuildCustomDomainEvent.Models;

namespace BuildCustomDomainEvent
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDomainEventDispatcher _eventDispatcher;

        public OrderService(ApplicationDbContext context, IDomainEventDispatcher eventDispatcher)
        {
            _context = context;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<Guid> CreateOrder(string customerName, decimal totalAmount)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = customerName,
                TotalAmount = totalAmount
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 触发领域事件
            await _eventDispatcher.Dispatch(this, new OrderCreatedEvent(order.Id));

            return order.Id;
        }
    }
}
