﻿using ErrorOr;
using Microsoft.Extensions.Logging;
using Ordering.Functions.Common;
using Ordering.Functions.Contracts;
using Ordering.Functions.Repositories;
using Ordering.Functions.Validation;

namespace Ordering.Functions.Services;

public class OrderingService(
    IOrderRepository orderRepository,
    ILogger<OrderingService> logger,
    IRequestValidator requestValidator)
    : IOrderingService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ILogger<OrderingService> _logger = logger;
    private readonly IRequestValidator _requestValidator = requestValidator;

    public async Task<ErrorOr<ProcessOrderResponse>> ProcessAsync(NewOrderRequest request)
    {
        _logger.LogDebug("Creating order {@Order}", request);

        var createResult = await _requestValidator.Validate(request)
            .ThenAsync(_ => _orderRepository.CreateAsync(request.ToOrder()));

        return createResult.Match<ErrorOr<ProcessOrderResponse>>(
            orderId =>
            {
                _logger.LogInformation("Order with id {OrderId} processed", orderId);
                return new ProcessOrderResponse(orderId);
            },
            error => error
        );
    }
}