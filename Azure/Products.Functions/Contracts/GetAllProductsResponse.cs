namespace Products.Functions.Contracts;

public record GetAllProductsResponse(IList<GetProductResponse> Products, int Count);