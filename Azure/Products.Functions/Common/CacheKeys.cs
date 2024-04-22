namespace Products.Functions.Common;

public static class CacheKeys
{
    public const string AllProductsKey = "all_products";
        
    public static string ProductKey(Guid id) => $"product_{id}";
}