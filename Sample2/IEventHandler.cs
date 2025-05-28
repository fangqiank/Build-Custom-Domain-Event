namespace Sample2
{
    public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task Handle(TEvent @event);
    }

    // 异步事件处理程序接口（可选，根据需求使用）
    public interface IAsyncEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent @event);
    }
}
