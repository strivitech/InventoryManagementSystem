using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Products.Functions.Common;
using Products.Functions.Domain;

namespace Products.Functions.Database;

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
    }
}