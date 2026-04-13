namespace ZRTCK.InventoryManagement.Persistence;

public class InventoryCategoryField
{
    public int Id { get; set; }
    public required string CategoryId { get; set; }
    public required string Code { get; set; } = null!;
    public required string Name { get; set; } = null!;
    public int Order { get; set; }
    public InventoryCategoryFieldType Type { get; set; }

    public InventoryCategory? Category { get; set; }
}
