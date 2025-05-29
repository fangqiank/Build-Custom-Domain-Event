using MediatRReplacedByDomainEvent.Data;

namespace MediatRReplacedByDomainEvent
{
    public class TransactionPipelineBehavior<TCommand, TResponse>(
        OrderDbContext context,
        ILogger<TransactionPipelineBehavior<TCommand, TResponse>> logger
        )
    : IPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    {
        public int Order => 300; // 最高值，最后执行

        public async Task<TResponse> Handle(
            TCommand command,
            CommandHandlerDelegate<TResponse> next,
            CancellationToken ct = default)
        {
            logger.LogDebug("Starting transaction for command: {CommandType}", typeof(TCommand).Name);

            await using var transaction = await context.Database.BeginTransactionAsync(ct);
            try
            {
                var response = await next();
                await transaction.CommitAsync(ct);
                return response;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
    }
}
