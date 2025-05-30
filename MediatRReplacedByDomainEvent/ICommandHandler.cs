namespace MediatRReplacedByDomainEvent
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command, CancellationToken ct = default);
    }
}
