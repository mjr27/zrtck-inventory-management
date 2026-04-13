using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZRTCK.InventoryManagement.Persistence.Configuration;

public class InventoryItemFieldConfiguration : IEntityTypeConfiguration<InventoryItemField>
{
    public void Configure(EntityTypeBuilder<InventoryItemField> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ItemId).HasMaxLength(36).IsRequired();
        builder.Property(x => x.CategoryFieldId).IsRequired();

        builder.HasOne(x => x.Item)
            .WithMany(x => x.Fields)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CategoryField)
            .WithMany()
            .HasForeignKey(x => x.CategoryFieldId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
