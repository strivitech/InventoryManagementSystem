using ErrorOr;
using Ordering.Functions.Domain;

namespace Ordering.Functions.Repositories;

public interface IOrderRepository
{
    Task<ErrorOr<Guid>> CreateAsync(Order order);
}