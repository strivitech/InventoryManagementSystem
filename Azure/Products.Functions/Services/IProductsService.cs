using ErrorOr;
using Products.Functions.Contracts;

namespace Products.Functions.Services;

public interface IProductsService
{
    Task<ErrorOr<CreateProductResponse>> CreateAsync(CreateProductRequest request);
    
    Task<ErrorOr<GetProductResponse>> GetAsync(GetProductRequest request);

    Task<ErrorOr<GetAllProductsResponse>> GetAllAsync();
    
    Task<ErrorOr<Updated>> UpdateAsync(UpdateProductRequest request);
    
    Task<ErrorOr<Deleted>> DeleteAsync(DeleteProductRequest request);
}