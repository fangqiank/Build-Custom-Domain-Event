namespace MediatRReplacedByDomainEvent
{
    public abstract class QueryPipelineBehavior<TQuery, TResult> : IQueryPipelineBehavior<TQuery, TResult>
    where TQuery : IQuery<TResult>
    {
        public abstract Task<TResult> HandleAsync(TQuery query, CancellationToken ct, Func<Task<TResult>> next);
    }
}
