using System.ComponentModel.DataAnnotations;

namespace MediatRReplacedByDomainEvent
{
    // 委托定义
    public delegate Task<TResponse> CommandHandlerDelegate<TResponse>();

    // 管道行为接口
    public interface IPipelineBehavior<in TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        int Order { get; }

        Task<TResponse> Handle(
            TCommand command,
            CommandHandlerDelegate<TResponse> next,
            CancellationToken ct = default);
    }
}
