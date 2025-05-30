namespace MediatRReplacedByDomainEvent
{
    public interface IQueryPipelineBehavior<TRequest, TResult>
    {
        Task<TResult> HandleAsync(TRequest request, CancellationToken ct, Func<Task<TResult>> next);
    }
}
