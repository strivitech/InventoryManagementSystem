using Products.Functions.Contracts;
using Products.Functions.Domain;

namespace Products.Functions.Common;

public static class MappingExtensions
{
    public static Product ToProduct(this CreateProductRequest request)
    {
        return new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Quantity = request.Quantity
        };
    }

    public static ProductQuantity ToProductQuantity(this ProductQuantityDto productQuantityDto) =>
        new(ProductId: productQuantityDto.ProductId, Quantity: productQuantityDto.Quantity);
}