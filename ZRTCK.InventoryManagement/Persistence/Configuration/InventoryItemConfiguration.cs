using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZRTCK.InventoryManagement.Persistence.Configuration;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.HasOne(item => item.InventoryInvoice)
            .WithMany(invoice => invoice.InventoryItems)
            .HasForeignKey(item => item.InventoryInvoiceId);

        builder.HasOne(item => item.Category)
            .WithMany(it => it.Items)
            .HasForeignKey(item => item.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}