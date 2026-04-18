using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZRTCK.InventoryManagement.Persistence.Configuration;

public class InventoryAssignmentRowConfiguration : IEntityTypeConfiguration<InventoryAssignmentRow>
{
    public void Configure(EntityTypeBuilder<InventoryAssignmentRow> builder)
    {
        builder.HasOne(row => row.Person)
            .WithMany(it => it.AssignmentRows)
            .HasForeignKey(row => row.PersonId);

        builder.HasOne(row => row.InventoryItem)
            .WithMany()
            .HasForeignKey(row => row.InventoryItemId);
    }
}