using ErrorOr;
using Microsoft.Extensions.Logging;
using Products.Functions.Common;
using Products.Functions.Contracts;
using Products.Functions.Repositories;
using Products.Functions.Validation;

namespace Products.Functions.Services;

public class WarehouseService(
    IWarehouseRepository warehouseRepository,
    IRequestValidator requestValidator,
    ILogger<WarehouseService> logger)
    : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository = warehouseRepository;
    private readonly IRequestValidator _requestValidator = requestValidator;
    private readonly ILogger<WarehouseService> _logger = logger;

    public async Task<ErrorOr<Success>> ReserveProductsAsync(ReserveProductsRequest request)
    {
        _logger.LogDebug("Reserving products {@Request}", request);

        var reserveProductsResult = await _requestValidator.Validate(request)
            .ThenAsync(_ => _warehouseRepository.ReserveProductsAsync(request.ProductQuantities
                .Select(pqd => pqd.ToProductQuantity()).ToList()));

        return reserveProductsResult.Match<ErrorOr<Success>>(
            _ => new Success(),
            error => error
        );
    }

    public async Task<ErrorOr<Success>> ReleaseProductsAsync(ReleaseProductsRequest request)
    {
        _logger.LogDebug("Releasing products {@Request}", request);

        var releaseProductsResult = await _requestValidator.Validate(request)
            .ThenAsync(_ => _warehouseRepository.ReleaseProductsAsync(request.ProductQuantities
                .Select(pqd => pqd.ToProductQuantity()).ToList()));

        return releaseProductsResult.Match<ErrorOr<Success>>(
            _ => new Success(),
            error => error
        );
    }
}