using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZRTCK.InventoryManagement.Persistence.Configuration;

public class InventoryCategoryConfiguration : IEntityTypeConfiguration<InventoryCategory>
{
    public void Configure(EntityTypeBuilder<InventoryCategory> builder)
    {
        builder.Property(it => it.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasKey(it => it.Id);
        builder.Property(it => it.Id).ValueGeneratedNever().HasMaxLength(32);
    }
}