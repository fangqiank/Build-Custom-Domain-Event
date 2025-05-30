using System.Diagnostics;
using System.Text.Json;

namespace MediatRReplacedByDomainEvent
{
    public class LoggingPipelineBehavior<TCommand>(
        ILogger<LoggingPipelineBehavior<TCommand>> logger
        ) : CommandPipelineBehavior<TCommand>
    where TCommand : ICommand
    {
        public override async Task HandleAsync(TCommand command, CancellationToken ct, Func<Task> next)
        {
            var commandName = typeof(TCommand).Name;

            // 序列化命令用于调试
            var commandJson = JsonSerializer.Serialize(command, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            logger.LogInformation("Handling command: {Command}\n{CommandJson}",
                commandName, commandJson);

            var sw = Stopwatch.StartNew();
            try
            {
                await next();
                logger.LogInformation("Command {Command} handled successfully in {Elapsed}ms",
                    commandName, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling command {Command} in {Elapsed}ms",
                    commandName, sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
