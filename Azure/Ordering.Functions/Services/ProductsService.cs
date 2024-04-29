using System.Net.Http.Json;
using System.Runtime.Serialization;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Ordering.Functions.Common;
using Ordering.Functions.Contracts;

namespace Ordering.Functions.Services;

public class ProductsService(HttpClient httpClient, ILogger<ProductsService> logger) : IProductsService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<ProductsService> _logger = logger;

    public async Task<ErrorOr<GetProductOverviewsResponse>> GetProductOverviewsAsync(GetProductOverviewsRequest request)
    {
        var queryParameterString = string.Join('&', request.ProductIds.Select(x => $"ProductIds={x}"));
        
        try
        {
            var response = await _httpClient.GetAsync($"GetProductOverviews?{queryParameterString}");
            
            if (!response.IsSuccessStatusCode)
            {
                return Errors.Order.GetProductOverviewsFailed();
            }

            var productOverviews = await response.Content.ReadFromJsonAsync<GetProductOverviewsResponse>()
                                   ?? throw new SerializationException("Failed to deserialize product overviews");

            return productOverviews;
        }
        catch (HttpRequestException  ex)
        {
            _logger.LogError(ex, "Failed to get product overviews");
            return Errors.Order.GetProductOverviewsFailed(); 
        }
    }
}