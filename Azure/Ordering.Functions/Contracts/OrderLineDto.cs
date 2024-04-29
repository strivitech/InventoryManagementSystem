using FluentValidation;

namespace Ordering.Functions.Contracts;

public record OrderLineDto(Guid ProductId, int Quantity, decimal Price);

public class OrderLineDtoValidator : AbstractValidator<OrderLineDto>
{
    public OrderLineDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}