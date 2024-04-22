namespace Products.Functions.Contracts;

public record GetProductResponse(Guid Id, string Name, string Description, decimal Price, int Quantity);
