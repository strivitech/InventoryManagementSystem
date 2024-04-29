using ErrorOr;
using Ordering.Functions.Common;
using Ordering.Functions.Contracts;

namespace Ordering.Functions.Services;

public class PreOrderingVerifier(IProductsService productsService) : IPreOrderingVerifier
{
    private readonly IProductsService _productsService = productsService;

    public async Task<ErrorOr<Success>> VerifyAsync(NewOrderRequest request)
    {
        var getProductOverviewsResult =
            await _productsService.GetProductOverviewsAsync(
                new GetProductOverviewsRequest(request.OrderLines.Select(x => x.ProductId).ToList()));

        return getProductOverviewsResult
            .Then(_ => CheckProductOverviewMatches(request, getProductOverviewsResult.Value));
    }

    private static ErrorOr<Success> CheckProductOverviewMatches(NewOrderRequest request,
        GetProductOverviewsResponse overviews)
    {
        var orderLinesDictionary = request.OrderLines.ToDictionary(x => x.ProductId, x => (x.Price, x.Quantity));
        var productOverviewsDictionary = overviews.ProductOverviews.ToDictionary(x => x.Id, x => (x.Price, x.Quantity));

        if (orderLinesDictionary.Count != productOverviewsDictionary.Count)
        {
            return Errors.Order.PreOrderingVerifierUnexpected();
        }

        return VerifyQuantitiesAllowed(orderLinesDictionary, productOverviewsDictionary)
            .Then(_ => VerifyPricesMatched(orderLinesDictionary, productOverviewsDictionary));
    }

    private static ErrorOr<Success> VerifyPricesMatched(
        Dictionary<Guid, (decimal Price, int Quantity)> orderLinesDictionary,
        Dictionary<Guid, (decimal Price, int Quantity)> productOverviewsDictionary)
    {
        var priceMismatchId = orderLinesDictionary
            .Where(kp => productOverviewsDictionary[kp.Key].Price != kp.Value.Price)
            .Select(kp => kp.Key)
            .FirstOrDefault();

        return priceMismatchId != Guid.Empty ? Errors.Order.PricesMismatch(priceMismatchId) : new Success();
    }

    private static ErrorOr<Success> VerifyQuantitiesAllowed(
        Dictionary<Guid, (decimal Price, int Quantity)> orderLinesDictionary,
        Dictionary<Guid, (decimal Price, int Quantity)> productOverviewsDictionary)
    {
        var quantityNotAllowedId = orderLinesDictionary
            .Where(kp => productOverviewsDictionary[kp.Key].Quantity < kp.Value.Quantity)
            .Select(kp => kp.Key)
            .FirstOrDefault();

        return quantityNotAllowedId != Guid.Empty
            ? Errors.Order.QuantitiesNotAllowed(quantityNotAllowedId)
            : new Success();
    }
}