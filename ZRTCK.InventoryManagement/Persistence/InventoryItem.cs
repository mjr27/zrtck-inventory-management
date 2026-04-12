using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

namespace ZRTCK.InventoryManagement.Persistence;

public class InventoryItem
{
    [Key] [StringLength(36)] public required string Id { get; set; }
    [StringLength(400)] public required string Name { get; set; }
    [StringLength(64)] public required string ProductCode { get; set; }
    public required int Year { get; set; }
    public required double Price { get; set; }

    public string? CategoryId { get; set; }
    public InventoryCategory? Category { get; set; }
    [StringLength(400)] public string? FactoryId { get; set; }

    public int InventoryInvoiceId { get; set; }
    public InventoryInvoice InventoryInvoice { get; set; } = null!;
    [NotMapped]
    public string? OriginalId { get; set; }
}