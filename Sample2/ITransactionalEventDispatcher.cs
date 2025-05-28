using Sample2;

public interface ITransactionalEventDispatcher
{
    Task CommitAsync();
    Task DispatchAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent;
}