using ErrorOr;

namespace Products.Functions.Common;

public static class Errors
{
    public static class Product
    {
        public static Error CreateFailed(Guid id) => Error.Failure("Product.CreateFailed", $"Failed to create product with id {id.ToString()}");
        
        public static Error NotFound(Guid id) => Error.Failure("Product.NotFound", $"Product with id {id.ToString()} not found");
        
        public static Error UpdateFailed(Guid id) => Error.Failure("Product.UpdateFailed", $"Failed to update product with id {id.ToString()}");
        
        public static Error DeleteFailed(Guid id) => Error.Failure("Product.DeleteFailed", $"Failed to delete product with id {id.ToString()}");
    }
}