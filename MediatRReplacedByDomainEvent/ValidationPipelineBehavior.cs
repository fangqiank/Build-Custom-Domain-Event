using FluentValidation;

namespace MediatRReplacedByDomainEvent
{
    public class ValidationPipelineBehavior<TCommand, TResponse>
    : IPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    {
        private readonly IValidator<TCommand>? _validator;

        public int Order => 200; // 在日志后执行

        public ValidationPipelineBehavior(IValidator<TCommand>? validator = null)
        {
            _validator = validator;
        }

        public async Task<TResponse> Handle(
            TCommand command,
            CommandHandlerDelegate<TResponse> next,
            CancellationToken ct = default)
        {
            if (_validator != null)
            {
                var result = await _validator.ValidateAsync(command, ct);

                if (!result.IsValid)
                {
                    throw new FluentValidation.ValidationException(
                        $"Command validation failed: {string.Join(", ", result.Errors)}");
                }
            }

            return await next();
        }
    }
}
