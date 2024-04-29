using System.Text.Json;
using ErrorOr;
using Microsoft.Extensions.Caching.Distributed;
using Products.Functions.Common;
using Products.Functions.Domain;

namespace Products.Functions.Repositories;

public class CachedProductsRepository(IProductsRepository productsRepository, IDistributedCache cache)
    : IProductsRepository
{
    private readonly IProductsRepository _productsRepository = productsRepository;
    private readonly IDistributedCache _cache = cache;
    private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(CacheLifeTimes.ProductsMinutes);

    public async Task<ErrorOr<Guid>> CreateAsync(Product product)
    {
        var createResponse = await _productsRepository.CreateAsync(product);

        return await createResponse.MatchAsync<ErrorOr<Guid>>(
            async result =>
            {
                await _cache.RemoveAsync(CacheKeys.AllProductsKey);
                return result;
            },
            errors => Task.FromResult<ErrorOr<Guid>>(errors)
        );
    }

    public async Task<ErrorOr<Product>> GetAsync(Guid id)
    {
        string cacheKey = CacheKeys.ProductKey(id);
        var cachedProduct = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedProduct))
        {
            return JsonSerializer.Deserialize<Product>(cachedProduct)!;
        }

        var getResponse = await _productsRepository.GetAsync(id);

        return await getResponse.MatchAsync<ErrorOr<Product>>(
            async product =>
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(product),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheLifetime
                    });
                return product;
            },
            errors => Task.FromResult<ErrorOr<Product>>(errors)
        );
    }

    public async Task<ErrorOr<List<Product>>> GetAsync(List<Guid> ids)
    {
        var products = new List<Product>();
        var missingIds = new List<Guid>();

        foreach (var id in ids)
        {
            string cacheKey = CacheKeys.ProductKey(id);
            var cachedProduct = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedProduct))
            {
                products.Add(JsonSerializer.Deserialize<Product>(cachedProduct)!);
            }
            else
            {
                missingIds.Add(id);
            }
        }

        if (missingIds.Any())
        {
            var getResponse = await _productsRepository.GetAsync(missingIds);

            return await getResponse.MatchAsync<ErrorOr<List<Product>>>(
                async newProducts =>
                {
                    foreach (var product in newProducts)
                    {
                        string cacheKey = CacheKeys.ProductKey(product.Id);
                        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(product),
                            new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = _cacheLifetime
                            });
                    }

                    products.AddRange(newProducts);
                    return products;
                },
                errors => Task.FromResult<ErrorOr<List<Product>>>(errors)
            );
        }

        return products;
    }

    public async Task<ErrorOr<List<Product>>> GetAllAsync()
    {
        string cacheKey = CacheKeys.AllProductsKey;
        var cachedProducts = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedProducts))
        {
            return JsonSerializer.Deserialize<List<Product>>(cachedProducts)!;
        }

        var getAllResponse = await _productsRepository.GetAllAsync();

        return await getAllResponse.MatchAsync<ErrorOr<List<Product>>>(
            async products =>
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(products),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = _cacheLifetime
                    });
                return products;
            },
            errors => Task.FromResult<ErrorOr<List<Product>>>(errors)
        );
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(Product product)
    {
        var updateResponse = await _productsRepository.UpdateAsync(product);

        return await updateResponse.MatchAsync<ErrorOr<Updated>>(
            async result =>
            {
                await _cache.RemoveAsync(CacheKeys.ProductKey(product.Id));
                await _cache.RemoveAsync(CacheKeys.AllProductsKey);
                return result;
            },
            errors => Task.FromResult<ErrorOr<Updated>>(errors)
        );
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid id)
    {
        var deleteResponse = await _productsRepository.DeleteAsync(id);

        return await deleteResponse.MatchAsync<ErrorOr<Deleted>>(
            async result =>
            {
                await _cache.RemoveAsync(CacheKeys.ProductKey(id));
                await _cache.RemoveAsync(CacheKeys.AllProductsKey);
                return result;
            },
            errors => Task.FromResult<ErrorOr<Deleted>>(errors)
        );
    }
}