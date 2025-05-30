using System.Windows.Input;

namespace MediatRReplacedByDomainEvent
{
    public interface ICommandDispatcher
    {
        Task SendAysnc<TCommand>(TCommand command, CancellationToken ct = default)
         where TCommand : ICommand;
    }

    //public class CommandDispatcher(
    //    IServiceProvider serviceProvider,
    //    ILogger<CommandDispatcher> logger
    //    ) : ICommandDispatcher
    //{

    //    public async Task<TResponse> Dispatch<TCommand, TResponse>(
    //        TCommand command, CancellationToken ct = default
    //        ) 
    //        where TCommand : ICommand<TResponse>
    //    {
    //        logger.LogInformation("Dispatching command: {CommandType}", typeof(TCommand).Name);

    //        try
    //        {
    //            // 获取命令处理器
    //            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();

    //            // 获取管道行为
    //            var behaviors = serviceProvider
    //                .GetServices<IPipelineBehavior<TCommand, TResponse>>()
    //                .OrderBy(b => b.Order)
    //                .ToList();

    //            CommandHandlerDelegate<TResponse> handlerDelegate = () =>
    //                handler.Handle(command, ct);

    //            foreach (var behavior in behaviors.AsEnumerable().Reverse())
    //            {
    //                var next = handlerDelegate;

    //                handlerDelegate = () => behavior.Handle(command, next, ct);
    //            }

    //            return await handlerDelegate();
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.LogError(ex, "Error dispatching command: {CommandType}", typeof(TCommand).Name);
    //            throw;
    //        }
    //    }
    //}
}
