using ErrorOr;
using Microsoft.Extensions.Logging;
using Products.Functions.Contracts;
using Products.Functions.Domain;
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

        var validationErrors = _requestValidator.Validate(request);

        if (validationErrors.Any())
        {
            return validationErrors;
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price
        };

        var createResponse = await _productsRepository.CreateAsync(product);
        
        return createResponse.Match<ErrorOr<CreateProductResponse>>(
            id => new CreateProductResponse(id),
            error => error
        );
    }

    public async Task<ErrorOr<GetProductResponse>> GetAsync(GetProductRequest request)
    {
        _logger.LogDebug("Getting product with id {Id}", request.Id);

        var validationErrors = _requestValidator.Validate(request);

        if (validationErrors.Any())
        {
            return validationErrors;
        }

        var getResponse = await _productsRepository.GetAsync(request.Id);

        return getResponse.Match<ErrorOr<GetProductResponse>>(
            p => new GetProductResponse(p.Id, p.Name, p.Description, p.Price),
            error => error
        );
    }

    public async Task<ErrorOr<GetAllProductsResponse>> GetAllAsync()
    {
        _logger.LogDebug("Getting all products");

        var getAllResponse = await _productsRepository.GetAllAsync();

        return getAllResponse.Match<ErrorOr<GetAllProductsResponse>>(
            products => new GetAllProductsResponse(products.Select(p =>
                new GetProductResponse(p.Id, p.Name, p.Description, p.Price)).ToList(), products.Count),
            error => error
        );
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(UpdateProductRequest request)
    {
        _logger.LogDebug("Updating product with id {Id}", request.Id);

        var validationErrors = _requestValidator.Validate(request);

        if (validationErrors.Any())
        {
            return validationErrors;
        }

        var getResponse = await _productsRepository.GetAsync(request.Id);

        if (getResponse.IsError)
        {
            return getResponse.Errors;
        }

        var product = getResponse.Value;
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;

        var updateResponse = await _productsRepository.UpdateAsync(product);

        return updateResponse.Match<ErrorOr<Updated>>(
            _ => new Updated(),
            error => error
        );
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(DeleteProductRequest request)
    {
        _logger.LogDebug("Deleting product with id {Id}", request.Id);

        var validationErrors = _requestValidator.Validate(request);

        if (validationErrors.Any())
        {
            return validationErrors;
        }

        var getResponse = await _productsRepository.GetAsync(request.Id);

        if (getResponse.IsError)
        {
            return getResponse.Errors;
        }

        var deleteResponse = await _productsRepository.DeleteAsync(getResponse.Value.Id);

        return deleteResponse.Match<ErrorOr<Deleted>>(
            _ => new Deleted(),
            error => error
        );
    }
}