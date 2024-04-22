using FluentValidation;

namespace Products.Functions.Contracts;

public record GetProductRequest(Guid Id);

public class GetProductRequestValidator : AbstractValidator<GetProductRequest>
{
    public GetProductRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}