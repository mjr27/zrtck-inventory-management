using System.ComponentModel;

namespace ZRTCK.InventoryManagement.Persistence;

public enum InventoryCategoryFieldType
{
    [Description("Рядок")]
    String,
    [Description("Захищений")]
    Protected,
    [Description("Текст")]
    Text
}