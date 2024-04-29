using ErrorOr;
using Ordering.Functions.Contracts;

namespace Ordering.Functions.Services;

public interface IProductsService
{   
    Task<ErrorOr<GetProductOverviewsResponse>> GetProductOverviewsAsync(GetProductOverviewsRequest request);
}