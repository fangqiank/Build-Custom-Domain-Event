using FluentValidation;
using FluentValidation.Results;

namespace MediatRReplacedByDomainEvent
{
    public class ValidationQueryPipelineBehavior<TQuery, TResult> : QueryPipelineBehavior<TQuery, TResult>
    where TQuery : IQuery<TResult>
    {
        private readonly IEnumerable<IValidator<TQuery>> _validators;
        private readonly ILogger<ValidationQueryPipelineBehavior<TQuery, TResult>> _logger;

        public ValidationQueryPipelineBehavior(
            IEnumerable<IValidator<TQuery>> validators,
            ILogger<ValidationQueryPipelineBehavior<TQuery, TResult>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public override async Task<TResult> HandleAsync(TQuery query, CancellationToken ct, Func<Task<TResult>> next)
        {
            // 如果没有验证器，直接继续
            if (!_validators.Any())
            {
                _logger.LogDebug("No validators found for query {Query}", typeof(TQuery).Name);
                return await next();
            }

            var context = new ValidationContext<TQuery>(query);
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

                _logger.LogWarning("Query validation failed for {Query}: {Errors}",
                    typeof(TQuery).Name, string.Join("; ", errors));

                throw new ValidationException($"Query validation failed: {string.Join(", ", errors)}");
            }

            return await next();
        }
    }
}
