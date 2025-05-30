using System.Diagnostics;

namespace MediatRReplacedByDomainEvent
{
    public class PerformancePipelineBehavior<TCommand> : CommandPipelineBehavior<TCommand>
    where TCommand : ICommand
    {
        private readonly ILogger<PerformancePipelineBehavior<TCommand>> _logger;
        private readonly long _warningThresholdMs;

        public PerformancePipelineBehavior(
            ILogger<PerformancePipelineBehavior<TCommand>> logger,
            IConfiguration config)
        {
            _logger = logger;
            _warningThresholdMs = config.GetValue<long>("Performance:WarningThresholdMs", 500);
        }

        public override async Task HandleAsync(TCommand command, CancellationToken ct, Func<Task> next)
        {
            var commandName = typeof(TCommand).Name;
            var sw = Stopwatch.StartNew();

            await next();

            var elapsed = sw.ElapsedMilliseconds;
            if (elapsed > _warningThresholdMs)
            {
                _logger.LogWarning("Command {Command} took {Elapsed}ms (threshold: {Threshold}ms)",
                    commandName, elapsed, _warningThresholdMs);
            }
        }
    }
}
