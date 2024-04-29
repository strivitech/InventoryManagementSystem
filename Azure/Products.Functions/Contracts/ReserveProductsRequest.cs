using FluentValidation;

namespace Products.Functions.Contracts;

public record ReserveProductsRequest(IList<ProductQuantityDto> ProductQuantities);

public class ReserveProductsRequestValidator : AbstractValidator<ReserveProductsRequest>
{
    public ReserveProductsRequestValidator(ProductQuantityValidator productQuantityValidator)
    {
        RuleFor(x => x.ProductQuantities).NotEmpty();
        RuleForEach(x => x.ProductQuantities).SetValidator(productQuantityValidator);
    }
}