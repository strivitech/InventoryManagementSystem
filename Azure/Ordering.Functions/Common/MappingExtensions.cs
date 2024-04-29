using Ordering.Functions.Domain;

namespace Ordering.Functions.Common;

public static class MappingExtensions
{
    public static Order ToOrder(this Contracts.NewOrderRequest request)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            TrackingNumber = Guid.NewGuid(),
            PartitionKey = $"Customer_{request.CustomerId}",
            OrderLines = request.OrderLines.Select(ToOrderLine).ToList(),
            ShippingAddressDto = request.ShippingAddress.ToAddress(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public static OrderLine ToOrderLine(this Contracts.OrderLineDto orderLineDto) =>
        new(ProductId: orderLineDto.ProductId, Quantity: orderLineDto.Quantity, Price: orderLineDto.Price);

    public static Address ToAddress(this Contracts.AddressDto addressDto) =>
        new(Street: addressDto.Street, City: addressDto.City, State: addressDto.State,
            ZipCode: addressDto.ZipCode);
}