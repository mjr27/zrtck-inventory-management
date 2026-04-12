using System.ComponentModel.DataAnnotations;

namespace ZRTCK.InventoryManagement.Persistence;

public class InventoryAssignmentRow
{
    [Key] public int Id { get; set; }
    public required int InventoryAssignmentId { get; set; }
    public InventoryAssignment InventoryAssignment { get; set; } = null!;
    public required int PersonId { get; set; }
    public Person Person { get; set; } = null!;
    [MaxLength(36)] public required string InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = null!;
}