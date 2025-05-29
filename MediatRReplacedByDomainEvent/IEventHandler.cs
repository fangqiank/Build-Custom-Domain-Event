namespace MediatRReplacedByDomainEvent
{
    // 领域事件接口
    public interface IDomainEvent { }

    // 事件处理器
    public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task Handle(TEvent @event, CancellationToken ct = default);
    }

}
