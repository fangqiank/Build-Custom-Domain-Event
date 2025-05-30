namespace MediatRReplacedByDomainEvent.Models
{
    public class Product : Entity<Guid>
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public int Stock { get; private set; }

        // 私有构造函数（EF Core需要）
        private Product() { }

        // 三个参数的构造函数（自动生成ID）
        public Product(string name, decimal price, int stock)
        {
            Id = Guid.NewGuid();
            Name = name;
            Price = price;
            Stock = stock;
        }

        // 四个参数的构造函数（指定ID）
        public Product(Guid id, string name, decimal price, int stock)
        {
            Id = id;
            Name = name;
            Price = price;
            Stock = stock;
        }

        public void ReduceStock(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive");
            if (quantity > Stock) throw new Exception("Insufficient stock");

            Stock -= quantity;
            RaiseEvent(new StockReducedEvent(Id, quantity));
        }

        public void Restock(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive");

            Stock += quantity;
            RaiseEvent(new StockRestockedEvent(Id, quantity));
        }
    }
}
