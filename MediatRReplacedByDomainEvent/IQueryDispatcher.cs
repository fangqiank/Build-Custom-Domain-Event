namespace MediatRReplacedByDomainEvent
{
    public interface IQueryDispatcher
    {
        Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct = default)
            where TQuery : IQuery<TResult>;
    }

    //public class QueryDispatcher(
    //        IServiceProvider serviceProvider,
    //        ILogger<QueryDispatcher> logger
    //    ) : IQueryDispatcher
    //{
    //    public async Task<TResponse> Dispatch<TQuery, TResponse>(
    //        TQuery query,
    //        CancellationToken ct = default)
    //        where TQuery : IQuery<TResponse>
    //    {
    //        try
    //        {
    //            var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();
    //            return await handler.Handle(query, ct);
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.LogError(ex, "Error dispatching query: {QueryType}", typeof(TQuery).Name);
    //            throw;
    //        }
    //    }
    //}
}
