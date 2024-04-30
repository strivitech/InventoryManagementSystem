using ErrorOr;
using Microsoft.Extensions.Logging;
using Products.Functions.Common;
using Products.Functions.Contracts;
using Products.Functions.Repositories;
using Products.Functions.Validation;

namespace Products.Functions.Services;

public class ProductsService(
    ILogger<ProductsService> logger,
    IRequestValidator requestValidator,
    IProductsRepository productsRepository)
    : IProductsService
{
    private readonly IProductsRepository _productsRepository = productsRepository;
    private readonly IRequestValidator _requestValidator = requestValidator;
    private readonly ILogger<ProductsService> _logger = logger;

    public async Task<ErrorOr<CreateProductResponse>> CreateAsync(CreateProductRequest request)
    {
        _logger.LogDebug("Creating product with name {Name}", request.Name);

        var createResult = await _requestValidator.Validate(request)
            .ThenAsync(_ => _productsRepository.CreateAsync(request.ToProduct()));

        return createResult.Match<ErrorOr<CreateProductResponse>>(
            id => new CreateProductResponse(id),
            error => error
        );
    }

    public async Task<ErrorOr<GetProductResponse>> GetAsync(GetProductRequest request)
    {
        _logger.LogDebug("Getting product with id {Id}", request.Id);

        var getResult = await _requestValidator.Validate(request)
            .ThenAsync(_ => _productsRepository.GetAsync(request.Id));

        return getResult.Match<ErrorOr<GetProductResponse>>(
            p => new GetProductResponse(p.Id, p.Name, p.Description, p.Price, p.Quantity),
            error => error
        );
    }

    public async Task<ErrorOr<GetAllProductsResponse>> GetAllAsync()
    {
        _logger.LogDebug("Getting all products");

        var getAllResult = await _productsRepository.GetAllAsync();

        return getAllResult.Match<ErrorOr<GetAllProductsResponse>>(
            products => new GetAllProductsResponse(products.Select(p =>
                new GetProductResponse(p.Id, p.Name, p.Description, p.Price, p.Quantity)).ToList(), products.Count),
            error => error
        );
    }

    public async Task<ErrorOr<GetProductOverviewsResponse>> GetProductOverviewsAsync(GetProductOverviewsRequest request)
    {
        _logger.LogDebug("Getting product overviews");

        var getProductOverviewsResult = await _requestValidator.Validate(request)
            .ThenAsync(_ => _productsRepository.GetAsync(request.ProductIds));

        return getProductOverviewsResult.Match<ErrorOr<GetProductOverviewsResponse>>(
            products => new GetProductOverviewsResponse(products.Select(p =>
                new ProductOverviewDto(p.Id, p.Price, p.Quantity)).ToList()),
            error => error
        );
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(UpdateProductRequest request)
    {
        _logger.LogDebug("Updating product with id {Id}", request.Id);

        var updatedProduct = await _requestValidator.Validate(request)
            .ThenAsync(_ => _productsRepository.GetAsync(request.Id))
            .Then(request.ToProduct);

        return await updatedProduct.MatchAsync(
            async product =>
            {
                var updateResult = await _productsRepository.UpdateAsync(product);

                return updateResult.Match<ErrorOr<Updated>>(
                    _ => new Updated(),
                    error => error
                );
            },
            error => Task.FromResult<ErrorOr<Updated>>(error));
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(DeleteProductRequest request)
    {
        _logger.LogDebug("Deleting product with id {Id}", request.Id);

        var getResult = await _requestValidator.Validate(request)
            .ThenAsync(_ => _productsRepository.GetAsync(request.Id));

        return await getResult.MatchAsync(
            async product =>
            {
                var deleteResult = await _productsRepository.DeleteAsync(product.Id);

                return deleteResult.Match<ErrorOr<Deleted>>(
                    _ => new Deleted(),
                    error => error
                );
            },
            error => Task.FromResult<ErrorOr<Deleted>>(error));
    }
}