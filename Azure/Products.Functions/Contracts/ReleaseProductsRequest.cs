using FluentValidation;

namespace Products.Functions.Contracts;

public record ReleaseProductsRequest(IList<ProductQuantityDto> ProductQuantities);

public class ReleaseProductsRequestValidator : AbstractValidator<ReleaseProductsRequest>
{
    public ReleaseProductsRequestValidator(ProductQuantityValidator productQuantityValidator)
    {
        RuleFor(x => x.ProductQuantities).NotEmpty();
        RuleForEach(x => x.ProductQuantities).SetValidator(productQuantityValidator);
    }
}