using ErrorOr;
using Products.Functions.Contracts;

namespace Products.Functions.Services;

public interface IWarehouseService
{
    Task<ErrorOr<Success>> ReserveProductsAsync(ReserveProductsRequest request);

    Task<ErrorOr<Success>> ReleaseProductsAsync(ReleaseProductsRequest request);
}