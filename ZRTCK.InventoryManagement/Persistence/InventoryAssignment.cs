using System.ComponentModel.DataAnnotations;

namespace ZRTCK.InventoryManagement.Persistence;

public class InventoryAssignment
{
    [Key] public int Id { get; private set; }
    public int? OrderId { get; set; }
    public DateOnly? OrderDate { get; set; }
    public List<InventoryAssignmentRow> AssignmentRows { get; set; } = [];
}