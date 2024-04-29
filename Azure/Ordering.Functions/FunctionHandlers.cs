using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Ordering.Functions.Contracts;
using Ordering.Functions.Services;
using TakeFromBody = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace Ordering.Functions;

public class FunctionHandlers(
    ILogger<FunctionHandlers> logger,
    IPreOrderingVerifier preOrderingVerifier,
    IOrderingService orderingService)
{
    private readonly ILogger<FunctionHandlers> _logger = logger;
    private readonly IPreOrderingVerifier _preOrderingVerifier = preOrderingVerifier;
    private readonly IOrderingService _orderingService = orderingService;

    [Function("VerifyOrder")]
    public async Task<IActionResult> VerifyOrderAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post")] [TakeFromBody]
        NewOrderRequest request)
    {
        _logger.LogDebug("VerifyOrder HTTP Post trigger function started processing a request");

        var verifyResponse = await _preOrderingVerifier.VerifyAsync(request);

        return verifyResponse.Match<IActionResult>(
            _ => new OkResult(),
            errors => new BadRequestObjectResult(errors)
        );
    }

    [Function("ProcessOrder")]
    public async Task<IActionResult> ProcessOrderAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post")] [TakeFromBody]
        NewOrderRequest request)
    {
        _logger.LogDebug("ProcessOrder HTTP Post trigger function started processing a request");

        var processResponse = await _orderingService.ProcessAsync(request);
        
        return processResponse.Match<IActionResult>(
            processOrderResponse => new OkObjectResult(processOrderResponse),
            errors => new BadRequestObjectResult(errors)
        );
    }
}