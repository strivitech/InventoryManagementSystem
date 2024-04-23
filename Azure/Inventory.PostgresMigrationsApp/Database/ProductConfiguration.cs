using Inventory.PostgresMigrationsApp.Common;
using Inventory.PostgresMigrationsApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.PostgresMigrationsApp.Database;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(Constants.MaxProductNameLength);
        
        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(Constants.MaxProductDescriptionLength);
        
        builder.ToTable(t => t
            .HasCheckConstraint("CK_Products_Quantity", "\"Quantity\" >= 0"));
    }
}