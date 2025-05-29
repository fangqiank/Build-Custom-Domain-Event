using FluentValidation;

namespace MediatRReplacedByDomainEvent
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("客户名称不能为空")
                .MaximumLength(100).WithMessage("客户名称长度不能超过100个字符");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("订单必须包含至少一个商品");

            RuleForEach(x => x.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.ProductName)
                        .NotEmpty().WithMessage("商品名称不能为空");

                    item.RuleFor(i => i.Quantity)
                        .GreaterThan(0).WithMessage("商品数量必须大于0");

                    item.RuleFor(i => i.UnitPrice)
                        .GreaterThan(0).WithMessage("商品单价必须大于0");
                });
        }
    }

}
