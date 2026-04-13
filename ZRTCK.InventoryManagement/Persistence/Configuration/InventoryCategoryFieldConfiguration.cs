using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZRTCK.InventoryManagement.Persistence.Configuration;

public class InventoryCategoryFieldConfiguration : IEntityTypeConfiguration<InventoryCategoryField>
{
    public void Configure(EntityTypeBuilder<InventoryCategoryField> builder)
    {
        builder.HasKey(it => it.Id);
        
        builder.Property(it => it.Code)
            .IsRequired()
            .HasMaxLength(32);
            
        builder.Property(it => it.Name)
            .IsRequired()
            .HasMaxLength(128);
            
        builder.Property(it => it.CategoryId)
            .IsRequired()
            .HasMaxLength(32);
            
        builder.HasOne(it => it.Category)
            .WithMany(it => it.Fields)
            .HasForeignKey(it => it.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Property(it => it.Type)
            .HasConversion<string>();
    }
}
