using FluentValidation;

namespace Products.Functions.Contracts;

public record ProductQuantityDto(Guid ProductId, int Quantity);

public class ProductQuantityValidator : AbstractValidator<ProductQuantityDto>
{
    public ProductQuantityValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}