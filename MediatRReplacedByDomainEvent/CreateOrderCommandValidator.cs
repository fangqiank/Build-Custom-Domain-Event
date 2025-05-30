using FluentValidation;
using MediatRReplacedByDomainEvent.Command;

namespace MediatRReplacedByDomainEvent
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerEmail)
                .NotEmpty().WithMessage("Customer email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("At least one order item is required");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be at least 1");
            });
        }
    }
}
