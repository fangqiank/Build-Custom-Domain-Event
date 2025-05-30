namespace MediatRReplacedByDomainEvent
{// 领域事件
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
