namespace MediatRReplacedByDomainEvent
{

    // 事件处理器
    public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken ct = default);
    }

}
