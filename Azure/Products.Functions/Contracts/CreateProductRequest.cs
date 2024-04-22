using FluentValidation;
using Products.Functions.Common;

namespace Products.Functions.Contracts;

public record CreateProductRequest(string Name, string Description, decimal Price, int Quantity);

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(Constants.MaxProductNameLength);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(Constants.MaxProductDescriptionLength);

        RuleFor(x => x.Price)
            .GreaterThan(0);
        
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0);
    }
}
