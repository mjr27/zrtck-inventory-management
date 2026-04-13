using System.ComponentModel.DataAnnotations;

namespace ZRTCK.InventoryManagement.Persistence;

public class InventoryItemField
{
    public int Id { get; set; }
    public required string ItemId { get; set; }
    public required int CategoryFieldId { get; set; }
    public string? Value { get; set; }

    public InventoryItem? Item { get; set; }
    public InventoryCategoryField? CategoryField { get; set; }
}
