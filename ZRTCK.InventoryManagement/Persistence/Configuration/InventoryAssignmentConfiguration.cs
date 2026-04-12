using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZRTCK.InventoryManagement.Persistence.Configuration;

public class InventoryAssignmentConfiguration : IEntityTypeConfiguration<InventoryAssignment>
{
    public void Configure(EntityTypeBuilder<InventoryAssignment> builder)
    {
        builder.HasMany(assignment => assignment.AssignmentRows)
            .WithOne(row => row.InventoryAssignment)
            .HasForeignKey(row => row.InventoryAssignmentId);
    }
}

