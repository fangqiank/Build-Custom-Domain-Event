namespace MediatRReplacedByDomainEvent
{
    public abstract class CommandPipelineBehavior<TCommand> : IPipelineBehavior<TCommand>
    where TCommand : ICommand
    {
        public abstract Task HandleAsync(TCommand command, CancellationToken ct, Func<Task> next);
    }
}
