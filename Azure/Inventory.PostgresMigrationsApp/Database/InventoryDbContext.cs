using Inventory.PostgresMigrationsApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace Inventory.PostgresMigrationsApp.Database;

public class InventoryDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();

    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}