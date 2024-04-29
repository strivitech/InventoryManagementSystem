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
        var validationErrors = _requestValidator.Validate(request);

        if (validationErrors.Any())
        {
            return validationErrors;
        }

        var reserveProductsResult =
            await _warehouseRepository.ReserveProductsAsync(request.ProductQuantities
                .Select(pqd => pqd.ToProductQuantity()).ToList());

        return reserveProductsResult.Match<ErrorOr<Success>>(
            success =>
            {
                _logger.LogInformation("Products Quantities {ProductQuantities} reserved successfully",
                    request.ProductQuantities);
                return new Success();
            },
            error => error
        );
    }

    public async Task<ErrorOr<Success>> ReleaseProductsAsync(ReleaseProductsRequest request)
    {
        var validationErrors = _requestValidator.Validate(request);

        if (validationErrors.Any())
        {
            return validationErrors;
        }

        var releaseProductsResult = await _warehouseRepository.ReleaseProductsAsync(request.ProductQuantities
            .Select(pqd => pqd.ToProductQuantity()).ToList());

        return releaseProductsResult.Match<ErrorOr<Success>>(
            success =>
            {
                _logger.LogInformation("Products Quantities {ProductQuantities} released successfully",
                    request.ProductQuantities);
                return new Success();
            },
            error => error
        );
    }
}