using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZRTCK.InventoryManagement.Persistence.Configuration;

public class InventoryInvoiceConfiguration : IEntityTypeConfiguration<InventoryInvoice>
{
    public void Configure(EntityTypeBuilder<InventoryInvoice> builder)
    {
    }
}

