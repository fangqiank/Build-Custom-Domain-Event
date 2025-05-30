using System.Diagnostics;
using System.Text.Json;

namespace MediatRReplacedByDomainEvent
{
    public class LoggingQueryPipelineBehavior<TQuery, TResult>(
         ILogger<LoggingQueryPipelineBehavior<TQuery, TResult>> logger
        ) : QueryPipelineBehavior<TQuery, TResult>
    where TQuery : IQuery<TResult>
    {
        public override async Task<TResult> HandleAsync(TQuery query, CancellationToken ct, Func<Task<TResult>> next)
        {
            var queryName = typeof(TQuery).Name;

            // 序列化查询用于调试
            var queryJson = JsonSerializer.Serialize(query, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            logger.LogInformation("Executing query: {Query}\n{QueryJson}",
                queryName, queryJson);

            var sw = Stopwatch.StartNew();
            try
            {
                var result = await next();

                // 限制结果日志大小
                var resultJson = JsonSerializer.Serialize(result, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (resultJson.Length > 1000)
                {
                    resultJson = resultJson.Substring(0, 1000) + "... (truncated)";
                }

                logger.LogInformation("Query {Query} executed successfully in {Elapsed}ms. Result: {Result}",
                    queryName, sw.ElapsedMilliseconds, resultJson);

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing query {Query} in {Elapsed}ms",
                    queryName, sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
