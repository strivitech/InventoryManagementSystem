using Microsoft.EntityFrameworkCore;
using Products.Functions.Domain;

namespace Products.Functions.Database;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}