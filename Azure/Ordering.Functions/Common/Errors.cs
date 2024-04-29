using ErrorOr;

namespace Ordering.Functions.Common;

public static class Errors
{
    public static class Order
    {
        public static Error QuantitiesNotAllowed(Guid productId) => Error.Failure("Product.QuantitiesNotAllowed",
            $"Quantities not allowed for product with id {productId.ToString()}");

        public static Error PricesMismatch(Guid productId) => Error.Failure("Product.PricesMismatch",
            $"Prices mismatch for product with id {productId.ToString()}");

        public static Error PreOrderingVerifierUnexpected() => Error.Unexpected("PreOrderingVerifier.Unexpected",
            "PreOrderingVerifier unexpected error");

        public static Error CreateFailed() => Error.Failure("Order.CreateFailed", "Failed to create order");
        
        public static Error GetProductOverviewsFailed() => Error.Failure("Order.GetProductOverviewsFailed", "Failed to get product overviews");
    }
}