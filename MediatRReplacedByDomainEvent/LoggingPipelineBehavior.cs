using System.Diagnostics;

namespace MediatRReplacedByDomainEvent
{
    public class LoggingPipelineBehavior<TCommand, TResponse>(
        ILogger<LoggingPipelineBehavior<TCommand, TResponse>> logger
        ): IPipelineBehavior<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public int Order = 100;

        int IPipelineBehavior<TCommand, TResponse>.Order => 1;

        public async Task<TResponse> Handle(
            TCommand command, 
            CommandHandlerDelegate<TResponse> next, 
            CancellationToken ct = default
            )
        {
            logger.LogInformation("开始处理命令: {CommandType}", typeof(TCommand).Name);
            logger.LogDebug("命令内容: {@Command}", command);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var response = await next();
                stopwatch.Stop();

                logger.LogInformation(
                    "命令处理完成: {CommandType}, 耗时: {ElapsedMilliseconds}ms",
                    typeof(TCommand).Name,
                    stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex,
                    "命令处理失败: {CommandType}, 耗时: {ElapsedMilliseconds}ms",
                    typeof(TCommand).Name,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
