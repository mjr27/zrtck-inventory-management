using System.ComponentModel.DataAnnotations;

namespace ZRTCK.InventoryManagement.Persistence;

public class Person
{
    
    [Key] public int Id { get; set; }
    public required string Rank { get; set; }
    public required string Position { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool IsPosition { get; set; }
    public bool IsObsolete { get; set; }
    
    public List<InventoryAssignmentRow> AssignmentRows { get; set; } = new();
}