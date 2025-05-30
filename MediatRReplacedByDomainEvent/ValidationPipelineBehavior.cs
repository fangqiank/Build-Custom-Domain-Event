using FluentValidation;
using FluentValidation.Results;

namespace MediatRReplacedByDomainEvent
{
    public class ValidationPipelineBehavior<TCommand> : CommandPipelineBehavior<TCommand>
     where TCommand : ICommand
    {
        private readonly IEnumerable<IValidator<TCommand>> _validators;
        private readonly ILogger<ValidationPipelineBehavior<TCommand>> _logger;

        public ValidationPipelineBehavior(
            IEnumerable<IValidator<TCommand>> validators,
            ILogger<ValidationPipelineBehavior<TCommand>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public override async Task HandleAsync(TCommand command, CancellationToken ct, Func<Task> next)
        {
            // 如果没有验证器，直接继续
            if (!_validators.Any())
            {
                _logger.LogDebug("No validators found for command {Command}", typeof(TCommand).Name);
                await next();
                return;
            }

            var context = new ValidationContext<TCommand>(command);
            var validationResults = new List<ValidationResult>();

            foreach (var validator in _validators)
            {
                var result = await validator.ValidateAsync(context, ct);
                if (!result.IsValid)
                {
                    validationResults.Add(result);
                }
            }

            if (validationResults.Any())
            {
                var errors = validationResults
                    .SelectMany(r => r.Errors)
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                    .ToList();

                _logger.LogWarning("Command validation failed for {Command}: {Errors}",
                    typeof(TCommand).Name, string.Join("; ", errors));

                throw new FluentValidation.ValidationException($"Command validation failed: {string.Join(", ", errors)}");
            }

            await next();
        }
    }

}
