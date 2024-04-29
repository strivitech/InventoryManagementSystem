using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Products.Functions.Common;
using Products.Functions.Database;
using Products.Functions.Domain;

namespace Products.Functions.Repositories;

public class ProductsRepository(ProductsDbContext dbContext, ILogger<ProductsRepository> logger) : IProductsRepository
{
    private readonly ProductsDbContext _dbContext = dbContext;
    private readonly ILogger<ProductsRepository> _logger = logger;

    public async Task<ErrorOr<Guid>> CreateAsync(Product product)
    {
        try
        {
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Created product with id {Id}", product.Id);
            return product.Id;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to create note");
            return Errors.Product.CreateFailed();
        }
    }

    public async Task<ErrorOr<Product>> GetAsync(Guid id)
    {
        var product = await _dbContext.Products.FindAsync(id);

        if (product is null)
        {
            return Errors.Product.NotFound(id);
        }
        
        _logger.LogDebug("Found product with id {Id}", id);

        return product;
    }

    public async Task<ErrorOr<List<Product>>> GetAsync(List<Guid> ids)
    {
        var products = await _dbContext.Products.Where(p => ids.Contains(p.Id)).ToListAsync();

        if (products.Count != ids.Count)
        {
            var missingIds = ids.Except(products.Select(p => p.Id)).ToList();
            _logger.LogWarning("Missing products with ids {Ids}", missingIds);
            return Errors.Product.NotFound(missingIds);
        }

        return products;
    }

    public async Task<ErrorOr<List<Product>>> GetAllAsync() => await _dbContext.Products.ToListAsync();

    public async Task<ErrorOr<Updated>> UpdateAsync(Product product)
    {
        try
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            return new Updated();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to update product with id {Id}", product.Id);
            return Errors.Product.UpdateFailed(product.Id);
        }
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid id)
    {
        var product = await _dbContext.Products.FindAsync(id);

        if (product is null)
        {
            return Errors.Product.NotFound(id);
        }

        _dbContext.Products.Remove(product);

        try
        {
            await _dbContext.SaveChangesAsync();
            return new Deleted();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to delete product with id {Id}", id);
            return Errors.Product.DeleteFailed(id);
        }
    }
}