using Microsoft.EntityFrameworkCore;
using Products.Functions.Domain;

namespace Products.Functions.Database;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}