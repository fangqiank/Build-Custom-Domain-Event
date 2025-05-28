using Sample2;
using Sample2.Data;

public class TransactionalEventDispatcher : IEventDispatcher, ITransactionalEventDispatcher
{
    private readonly IEventDispatcher _inner;
    private readonly OrderDbContext _dbContext;
    private readonly List<IDomainEvent> _pendingEvents = new();

    public TransactionalEventDispatcher(
        IEventDispatcher inner,
        OrderDbContext dbContext)
    {
        _inner = inner;
        _dbContext = dbContext;
    }

    public async Task DispatchAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent
    {
        if (_dbContext.Database.CurrentTransaction != null)
        {
            // 暂存事件
            _pendingEvents.Add(@event);
        }
        else
        {
            // 直接分发事件
            await _inner.DispatchAsync(@event);
        }
    }

    public async Task CommitAsync()
    {
        if (_dbContext.Database.CurrentTransaction == null) return;

        await _dbContext.Database.CommitTransactionAsync();

        // 分发暂存事件
        foreach (var evt in _pendingEvents)
        {
            await _inner.DispatchAsync(evt);
        }
        _pendingEvents.Clear();
    }
}