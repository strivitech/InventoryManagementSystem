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
    
    public static Product ToProduct(this UpdateProductRequest request, Product product)
    {
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Quantity = request.Quantity;
        return product;
    }

    public static ProductQuantity ToProductQuantity(this ProductQuantityDto productQuantityDto) =>
        new(ProductId: productQuantityDto.ProductId, Quantity: productQuantityDto.Quantity);
}