using Microsoft.EntityFrameworkCore;
using Products.Functions.Domain;

namespace Products.Functions.Database;

public class ProductsDbContext(DbContextOptions<ProductsDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}