using ErrorOr;
using Ordering.Functions.Contracts;

namespace Ordering.Functions.Services;

public interface IOrderingService
{   
    Task<ErrorOr<ProcessOrderResponse>> ProcessAsync(NewOrderRequest request);
}