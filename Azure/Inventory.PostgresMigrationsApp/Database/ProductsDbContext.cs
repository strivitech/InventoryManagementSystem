using Inventory.PostgresMigrationsApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace Inventory.PostgresMigrationsApp.Database;

public class ProductsDbContext(DbContextOptions<ProductsDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}