using FluentValidation;
using Products.Functions.Common;

namespace Products.Functions.Contracts;

public record UpdateProductRequest(Guid Id, string Name, string Description, decimal Price, int Quantity);

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        
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