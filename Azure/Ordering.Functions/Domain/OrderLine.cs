namespace Ordering.Functions.Domain;

public record OrderLine(
    Guid ProductId,
    int Quantity,
    decimal Price);