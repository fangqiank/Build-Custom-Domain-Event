namespace MediatRReplacedByDomainEvent
{
    public interface IPipelineBehavior<TRequest>
    {
        Task HandleAsync(TRequest request, CancellationToken ct, Func<Task> next);
    }
}
