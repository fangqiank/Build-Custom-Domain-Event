using MediatRReplacedByDomainEvent.Data;

namespace MediatRReplacedByDomainEvent
{
    public class TransactionPipelineBehavior<TCommand>(
        AppDbContext dbContext,
        ILogger<TransactionPipelineBehavior<TCommand>> logger
        ) : CommandPipelineBehavior<TCommand>
     where TCommand : ICommand
    {
        public override async Task HandleAsync(TCommand command, CancellationToken ct, Func<Task> next)
        {
            // 检查数据库是否支持事务
            var databaseProvider = dbContext.Database.ProviderName;
            var isInMemoryDatabase = databaseProvider == "Microsoft.EntityFrameworkCore.InMemory";

            if (isInMemoryDatabase)
            {
                logger.LogWarning("Skipping transaction for command {Command} as in-memory database doesn't support transactions",
                    typeof(TCommand).Name);
                await next();
                return;
            }

            if (dbContext.Database.CurrentTransaction != null)
            {
                await next();
                return;
            }

            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
            logger.LogInformation("Begin transaction for command {Command}", typeof(TCommand).Name);

            try
            {
                await next();
                await transaction.CommitAsync(ct);
                logger.LogInformation("Transaction committed for command {Command}", typeof(TCommand).Name);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                logger.LogError("Transaction rolled back for command {Command}", typeof(TCommand).Name);
                throw;
            }
        }
    }
}
