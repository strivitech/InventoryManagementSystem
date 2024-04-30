using System.Net;
using ErrorOr;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Ordering.Functions.Common;
using Ordering.Functions.Domain;

namespace Ordering.Functions.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ILogger<OrderRepository> _logger;
    private readonly Container _container;

    public OrderRepository(ILogger<OrderRepository> logger, CosmosClient cosmosClient)  
    {
        _logger = logger;
        var database = cosmosClient.GetDatabase(DatabaseConstants.DatabaseName);
        _container = database.GetContainer(DatabaseConstants.ContainerName);
    }
    
    public async Task<ErrorOr<Guid>> CreateAsync(Order order)
    {
        try
        {
            ItemResponse<Order> createResponse = await _container.CreateItemAsync(order, new PartitionKey(order.PartitionKey));
            if (createResponse.StatusCode != HttpStatusCode.Created)
            {   
                _logger.LogError("Failed to create order with id {OrderId}", order.Id);
                return Errors.Order.CreateFailed();
            }
            
            _logger.LogInformation("Order created with id {OrderId}", order.Id);
            return order.Id;
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "Failed to create order with id {OrderId}", order.Id);
            return Errors.Order.CreateFailed();
        }
    }
}