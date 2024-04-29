using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Products.Functions.Common;
using Products.Functions.Database;
using Products.Functions.Domain;

namespace Products.Functions.Repositories;

public class WarehouseRepository(ProductsDbContext dbContext, ILogger<WarehouseRepository> logger)
    : IWarehouseRepository
{
    private readonly ProductsDbContext _dbContext = dbContext;
    private readonly ILogger<WarehouseRepository> _logger = logger;

    public async Task<ErrorOr<Success>> ReserveProductsAsync(IList<ProductQuantity> productQuantities)
    {   
        var productsToReserve = _dbContext.Products
            .Where(p => productQuantities.Select(pq => pq.ProductId).Contains(p.Id))
            .ToList();

        var productQuantitiesDictionary = productQuantities.ToDictionary(pq => pq.ProductId, pq => pq.Quantity);

        foreach (var productToReserve in productsToReserve)
        {
            productToReserve.Quantity -= productQuantitiesDictionary[productToReserve.Id];

            if (productToReserve.Quantity < 0)
            {
                return Errors.Product.QuantityNotAvailable(productToReserve.Id);
            }
        }

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to save changes while reserving products");
            return Errors.Product.ReserveFailed();
        }
        
        return new Success();
    }

    public async Task<ErrorOr<Success>> ReleaseProductsAsync(IList<ProductQuantity> productQuantities)
    {
        var productsToRelease = _dbContext.Products
            .Where(p => productQuantities.Select(pq => pq.ProductId).Contains(p.Id))
            .ToList();

        var productQuantitiesDictionary = productQuantities.ToDictionary(pq => pq.ProductId, pq => pq.Quantity);

        foreach (var productToRelease in productsToRelease)
        {
            productToRelease.Quantity += productQuantitiesDictionary[productToRelease.Id];
        }

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to save changes while releasing products");
            return Errors.Product.ReleaseFailed();
        }
        
        return new Success();
    }
}