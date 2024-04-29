using ErrorOr;
using Products.Functions.Domain;

namespace Products.Functions.Repositories;

public interface IProductsRepository
{
    Task<ErrorOr<Guid>> CreateAsync(Product product);

    Task<ErrorOr<Product>> GetAsync(Guid id);
    
    Task<ErrorOr<List<Product>>> GetAsync(List<Guid> ids);

    Task<ErrorOr<List<Product>>> GetAllAsync();

    Task<ErrorOr<Updated>> UpdateAsync(Product product);

    Task<ErrorOr<Deleted>> DeleteAsync(Guid id);
}