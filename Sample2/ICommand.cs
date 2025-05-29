namespace Sample2
{
    // 命令接口
    public interface ICommand<TResponse> { }

    // 命令处理器
    public interface ICommandHandler<in TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        Task<TResponse> Handle(TCommand command, CancellationToken ct = default);
    }
}
