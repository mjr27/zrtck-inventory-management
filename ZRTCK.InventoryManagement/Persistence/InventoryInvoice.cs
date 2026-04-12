using System.ComponentModel.DataAnnotations;

namespace ZRTCK.InventoryManagement.Persistence;

public class InventoryInvoice
{
    [Key] public int Id { get; init; }
    public required int InvoiceId { get; set; }
    public required DateOnly InvoiceDate { get; set; }

    public List<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
}