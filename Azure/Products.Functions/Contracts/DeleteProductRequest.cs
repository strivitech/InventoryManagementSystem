using FluentValidation;

namespace Products.Functions.Contracts;

public record DeleteProductRequest(Guid Id);

public class DeleteProductRequestValidator : AbstractValidator<DeleteProductRequest>
{
    public DeleteProductRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}