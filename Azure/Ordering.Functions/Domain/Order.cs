namespace Ordering.Functions.Domain;

public record Order
{
    public Guid Id { get; init; }
    public string CustomerId { get; init; } = null!;
    public Guid? TrackingNumber { get; init; }
    public string PartitionKey { get; init; } = null!;
    public List<OrderLine> OrderLines { get; init; } = null!;
    public Address ShippingAddressDto { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
}