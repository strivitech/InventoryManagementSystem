using FluentValidation;

namespace Ordering.Functions.Contracts;

public record GetProductOverviewsRequest(List<Guid> ProductIds);

public class GetProductOverviewsRequestValidator : AbstractValidator<GetProductOverviewsRequest>
{
    public GetProductOverviewsRequestValidator()
    {
        RuleFor(x => x.ProductIds).NotEmpty();
    }
}