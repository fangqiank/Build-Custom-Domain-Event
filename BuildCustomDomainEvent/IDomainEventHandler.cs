namespace BuildCustomDomainEvent
{
    public interface IDomainEventHandler<T> where T : class
    {
        Task Handle(T @event);
    }
}
