using ErrorOr;

namespace Products.Functions.Common;

public static class Errors
{
    public static class Product
    {
        public static Error CreateFailed() => Error.Failure("Product.CreateFailed", "Failed to create product");
        
        public static Error NotFound(Guid id) => Error.Failure("Product.NotFound", $"Product with id {id.ToString()} not found");
        
        public static Error NotFound(IEnumerable<Guid> ids) => Error.Failure("Products.NotFound", $"Product with ids {string.Join(", ", ids)} not found");
        
        public static Error UpdateFailed(Guid id) => Error.Failure("Product.UpdateFailed", $"Failed to update product with id {id.ToString()}");
        
        public static Error DeleteFailed(Guid id) => Error.Failure("Product.DeleteFailed", $"Failed to delete product with id {id.ToString()}");
        
        public static Error QuantityNotAvailable(Guid id) => Error.Failure("Product.QuantityNotAvailable", $"Product with id {id.ToString()} does not have enough quantity available");
        
        public static Error ReserveFailed() => Error.Failure("Product.ReserveFailed", "Failed to reserve products unexpectedly");
        
        public static Error ReleaseFailed() => Error.Failure("Product.ReleaseFailed", "Failed to release products unexpectedly");
    }
}