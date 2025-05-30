using System.Diagnostics;

namespace MediatRReplacedByDomainEvent
{
    public class PerformanceQueryPipelineBehavior<TQuery, TResult> : QueryPipelineBehavior<TQuery, TResult>
    where TQuery : IQuery<TResult>
    {
        private readonly ILogger<PerformanceQueryPipelineBehavior<TQuery, TResult>> _logger;
        private readonly long _warningThresholdMs;

        public PerformanceQueryPipelineBehavior(
            ILogger<PerformanceQueryPipelineBehavior<TQuery, TResult>> logger,
            IConfiguration config)
        {
            _logger = logger;
            _warningThresholdMs = config.GetValue<long>("Performance:WarningThresholdMs", 500);
        }

        public override async Task<TResult> HandleAsync(TQuery query, CancellationToken ct, Func<Task<TResult>> next)
        {
            var queryName = typeof(TQuery).Name;
            var sw = Stopwatch.StartNew();

            var result = await next();

            var elapsed = sw.ElapsedMilliseconds;
            if (elapsed > _warningThresholdMs)
            {
                _logger.LogWarning("Query {Query} took {Elapsed}ms (threshold: {Threshold}ms)",
                    queryName, elapsed, _warningThresholdMs);
            }

            return result;
        }
    }
}
