using Microsoft.EntityFrameworkCore;
using Sample2.Models;

namespace Sample2.Data
{
    public class OrderDbContext:DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Order> Orders => Set<Order>();
    }
}
