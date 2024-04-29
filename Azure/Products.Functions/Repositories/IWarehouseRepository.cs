using ErrorOr;
using Products.Functions.Domain;

namespace Products.Functions.Repositories;

public interface IWarehouseRepository
{
    Task<ErrorOr<Success>> ReserveProductsAsync(IList<ProductQuantity> productQuantities);

    Task<ErrorOr<Success>> ReleaseProductsAsync(IList<ProductQuantity> productQuantities);
}