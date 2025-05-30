using System.Diagnostics;
using System.Windows.Input;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MediatRReplacedByDomainEvent
{
    public class Dispatcher(
        IServiceProvider serviceProvider,
        ILogger<Dispatcher> logger
        ) : ICommandDispatcher, IQueryDispatcher, IEventDispatcher
    {
        public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken ct = default)
            where TQuery : IQuery<TResult>
        {
            logger.LogDebug("Executing query: {QueryName}", typeof(TQuery).Name);

            try
            {
                // 获取查询处理器
                var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();

                // 获取管道行为
                var behaviors = serviceProvider
                    .GetServices<IQueryPipelineBehavior<TQuery, TResult>>()
                    .ToList();

                // 创建管道委托
                Func<Task<TResult>> pipeline = () => handler.HandleAsync(query, ct);

                // 构建管道（从内到外包装）
                foreach (var behavior in behaviors.AsEnumerable().Reverse())
                {
                    var next = pipeline;
                    pipeline = () => behavior.HandleAsync(query, ct, next);
                }

                // 执行管道
                return await pipeline();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling query {QueryName}", typeof(TQuery).Name);
                throw;
            }
        }

        public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken ct = default)
        {
            var eventType = domainEvent.GetType();
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            var handlers = serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod("HandleAsync");
                await (Task)method.Invoke(handler, new object[] { domainEvent, ct });
            }
        }

        public async Task SendAysnc<TCommand>(TCommand command, CancellationToken ct = default) 
            where TCommand : ICommand
        {
            var commandName = typeof(TCommand).Name;
            logger.LogInformation("Dispatching command: {CommandName}", commandName);
            
            var sw = Stopwatch.StartNew();
            try
            {
                // 获取命令处理器
                var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();

                // 获取管道行为
                var behaviors = serviceProvider
                    .GetServices<IPipelineBehavior<TCommand>>()
                    .ToList();

                // 创建管道委托
                Func<Task> pipeline = () => handler.HandleAsync(command, ct);

                // 构建管道（从内到外包装）
                foreach (var behavior in behaviors.AsEnumerable().Reverse())
                {
                    var next = pipeline;
                    pipeline = () => behavior.HandleAsync(command, ct, next);
                }

                // 执行管道
                await pipeline();

                logger.LogInformation("Command {CommandName} handled successfully in {Elapsed}ms",
                    commandName, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling command {CommandName} in {Elapsed}ms",
                    commandName, sw.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
