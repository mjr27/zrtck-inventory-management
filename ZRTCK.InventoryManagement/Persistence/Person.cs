using System.ComponentModel.DataAnnotations;

namespace ZRTCK.InventoryManagement.Persistence;

public class Person
{
    public required string Rank { get; set; }
    public required string Position { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool IsObsolete { get; set; }
    [Key] public int Id { get; set; }
}