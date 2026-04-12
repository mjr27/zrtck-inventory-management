namespace ZRTCK.InventoryManagement.Persistence;

public class InventoryCategory
{
    public required string Id { get; set; }
    public required string Name { get; set; } = null!;
    
    public List<InventoryItem> Items { get; set; } = [];
}