using MediatRReplacedByDomainEvent.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MediatRReplacedByDomainEvent.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IEventDispatcher _eventDispatcher;

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Product> Products => Set<Product>();

        public AppDbContext(DbContextOptions<AppDbContext> options, IEventDispatcher eventDispatcher)
            : base(options)
        {
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // 忽略内存数据库的事务警告
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);

                // 配置拥有的集合
                entity.OwnsMany(o => o.Items, item =>
                {
                    // 显式配置影子属性
                    item.WithOwner().HasForeignKey("OrderId");
                    item.Property<Guid>("Id");
                    item.HasKey("Id");

                    // 显式映射所有属性
                    item.Property(i => i.ProductId).IsRequired();
                    item.Property(i => i.ProductName).IsRequired().HasMaxLength(100);
                    item.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
                    item.Property(i => i.Quantity).IsRequired();

                    // 忽略计算属性
                    item.Ignore(i => i.TotalPrice);
                });
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Stock).IsRequired();
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            // 获取所有包含领域事件的实体
            var entitiesWithEvents = ChangeTracker.Entries<Entity<Guid>>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToArray();

            // 先保存变更以获取ID
            var result = await base.SaveChangesAsync(ct);

            // 分发领域事件
            foreach (var entity in entitiesWithEvents)
            {
                foreach (var domainEvent in entity.DomainEvents)
                {
                    await _eventDispatcher.PublishAsync(domainEvent, ct);
                }
                entity.ClearEvents();
            }

            return result;
        }
    }
}
