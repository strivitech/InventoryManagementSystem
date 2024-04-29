using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Products.Functions.Contracts;
using Products.Functions.Services;
using TakeFromBody = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace Products.Functions;

public class FunctionHandlers(
    ILogger<FunctionHandlers> logger,
    IProductsService productsService,
    IWarehouseService warehouseService)
{
    private readonly ILogger<FunctionHandlers> _logger = logger;
    private readonly IProductsService _productsService = productsService;
    private readonly IWarehouseService _warehouseService = warehouseService;

    [Function("CreateProduct")]
    public async Task<IActionResult> CreateProductAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post")] [TakeFromBody]
        CreateProductRequest request)
    {
        _logger.LogDebug("CreateProduct HTTP Post trigger function started processing a request");

        var createResponse = await _productsService.CreateAsync(request);

        return createResponse.Match<IActionResult>(
            value => new OkObjectResult(value),
            errors => new BadRequestObjectResult(errors)
        );
    }

    [Function("GetProduct")]
    public async Task<IActionResult> GetProductAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetProduct/{id:guid}")]
        HttpRequest req, string id)
    {
        var request = new GetProductRequest(Guid.Parse(id));

        _logger.LogDebug("GetProduct HTTP Get trigger function started processing a request");

        var getResponse = await _productsService.GetAsync(request);

        return getResponse.Match<IActionResult>(
            p => new OkObjectResult(p),
            _ => new NotFoundResult()
        );
    }

    [Function("GetProductOverviews")]
    public async Task<IActionResult> GetProductOverviewsAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get")]
        HttpRequest req)
    {
        _logger.LogDebug("GetProductOverviews HTTP Get trigger function started processing a request");

        var listOfIds = req.Query[nameof(GetProductOverviewsRequest.ProductIds)].Select(Guid.Parse!).ToList();

        var getOverviewsResponse =
            await _productsService.GetProductOverviewsAsync(new GetProductOverviewsRequest(listOfIds));

        return getOverviewsResponse.Match<IActionResult>(
            overviews => overviews.ProductOverviews.Any()
                ? new OkObjectResult(overviews)
                : new EmptyResult(),
            errors => new BadRequestObjectResult(errors)
        );
    }

    [Function("GetAllProducts")]
    public async Task<IActionResult> GetAllProductsAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get")]
        HttpRequest req)
    {
        _logger.LogDebug("GetAllProducts HTTP Get trigger function started processing a request");

        var getAllResponse = await _productsService.GetAllAsync();

        return getAllResponse.Match<IActionResult>(
            productsResponse => productsResponse.Products.Any()
                ? new OkObjectResult(productsResponse)
                : new
                    EmptyResult(),
            _ => new NotFoundResult()
        );
    }

    [Function("UpdateProduct")]
    public async Task<IActionResult> UpdateProductAsync(
        [HttpTrigger(AuthorizationLevel.Function, "put")] [TakeFromBody]
        UpdateProductRequest request)
    {
        _logger.LogDebug("UpdateProduct HTTP Put trigger function started processing a request");

        var updateResponse = await _productsService.UpdateAsync(request);

        return updateResponse.Match<IActionResult>(
            _ => new OkResult(),
            errors => new BadRequestObjectResult(errors)
        );
    }

    [Function("DeleteProduct")]
    public async Task<IActionResult> DeleteProductAsync(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteProduct/{id:guid}")]
        HttpRequest req, string id)
    {
        var request = new DeleteProductRequest(Guid.Parse(id));

        _logger.LogDebug("DeleteProduct HTTP Delete trigger function started processing a request");

        var deleteResponse = await _productsService.DeleteAsync(request);

        return deleteResponse.Match<IActionResult>(
            _ => new OkResult(),
            errors => new BadRequestObjectResult(errors)
        );
    }

    [Function("ReserveProducts")]
    public async Task<IActionResult> ReserveProductsAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post")] [TakeFromBody]
        ReserveProductsRequest request)
    {
        _logger.LogDebug("ReserveProducts HTTP Post trigger function started processing a request");

        var reserveResponse = await _warehouseService.ReserveProductsAsync(request);

        return reserveResponse.Match<IActionResult>(
            _ => new OkResult(),
            errors => new BadRequestObjectResult(errors)
        );
    }

    [Function("ReleaseProducts")]
    public async Task<IActionResult> ReleaseProductsAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post")] [TakeFromBody]
        ReleaseProductsRequest request)
    {
        _logger.LogDebug("ReleaseProducts HTTP Post trigger function started processing a request");

        var releaseResponse = await _warehouseService.ReleaseProductsAsync(request);

        return releaseResponse.Match<IActionResult>(
            _ => new OkResult(),
            errors => new BadRequestObjectResult(errors)
        );
    }
}