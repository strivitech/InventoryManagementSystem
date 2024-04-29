using ErrorOr;
using Ordering.Functions.Contracts;

namespace Ordering.Functions.Services;

public interface IPreOrderingVerifier
{
    Task<ErrorOr<Success>> VerifyAsync(NewOrderRequest request);
}