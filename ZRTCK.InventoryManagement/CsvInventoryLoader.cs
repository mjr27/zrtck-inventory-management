using System.Globalization;
using System.Text;
using ZRTCK.InventoryManagement.Persistence;

namespace ZRTCK.InventoryManagement;

public static class CsvInventoryLoader
{
    /// <summary>
    /// Parses maino.csv and seeds the database with invoices and items grouped by InvoiceId.
    /// Should be called only when the database is freshly created.
    /// </summary>
    public static async Task LoadFromCsvAsync(InventoryDbContext context, string? csvPath = null)
    {
        csvPath ??= FindCsvFile();

        if (csvPath is null)
        {
            Console.WriteLine("CsvInventoryLoader: maino.csv not found — skipping import.");
            return;
        }

        Console.WriteLine($"CsvInventoryLoader: loading from {csvPath}");

        try
        {
            // Seed categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new InventoryCategory { Id = "Computer", Name = "Комп'ютер" },
                    new InventoryCategory { Id = "Printer", Name = "Принтер" },
                    new InventoryCategory { Id = "Video", Name = "Відео" },
                    new InventoryCategory { Id = "Network", Name = "Мережа" },
                    new InventoryCategory { Id = "Oberig", Name = "Оберіг" },
                    new InventoryCategory { Id = "Radio", Name = "Радіо" },
                    new InventoryCategory { Id = "Phone", Name = "Телефон" }
                );
                await context.SaveChangesAsync();
            }

            var rows = ParseCsv(csvPath);

            // Deduplicate by item Id (the CSV contains duplicate rows at the end)
            var uniqueRows = rows
                .GroupBy(r => r.ItemId)
                .Select(g => g.First())
                .ToList();

            // Group by InvoiceId (each InvoiceId maps to exactly one date)
            var invoiceGroups = uniqueRows
                .GroupBy(r => r.InvoiceId)
                .OrderBy(g => g.First().InvoiceDate);

            foreach (var group in invoiceGroups)
            {
                var firstRow = group.First();

                var invoice = new InventoryInvoice
                {
                    InvoiceId = firstRow.InvoiceId,
                    InvoiceDate = firstRow.InvoiceDate
                };

                foreach (var row in group)
                {
                    invoice.InventoryItems.Add(new InventoryItem
                    {
                        Id = row.ItemId,
                        Name = row.Name,
                        ProductCode = row.ProductCode,
                        Year = row.Year,
                        Price = row.Price,
                        FactoryId = string.IsNullOrWhiteSpace(row.FactoryId) ? null : row.FactoryId,
                        CategoryId = row.CategoryId
                    });
                }

                context.Invoices.Add(invoice);
            }

            await context.SaveChangesAsync();
            Console.WriteLine($"CsvInventoryLoader: imported {uniqueRows.Count} items across the loaded invoices.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CsvInventoryLoader: error during import — {ex.Message}");
            throw;
        }
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private static string? FindCsvFile()
    {
        // Primary: copied next to the binary via .csproj <Content> item

        var next = Path.Combine(AppContext.BaseDirectory, "maino.csv");
        if (File.Exists(next)) return next;
        next = Path.Combine(AppContext.BaseDirectory, "..", "maino.csv");
        if (File.Exists(next)) return next;
        // Fallback: walk up to the solution root (useful when running with `dotnet run`)
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "maino.csv");
            if (File.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }

        return null;
    }

    private record CsvRow(
        string ProductCode,
        string Name,
        double Price,
        int Year,
        string ItemId,
        string FactoryId,
        int InvoiceId,
        DateOnly InvoiceDate,
        string? CategoryId);

    private static List<CsvRow> ParseCsv(string path)
    {
        var rows = new List<CsvRow>();

        foreach (var line in File.ReadLines(path, Encoding.UTF8))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var fields = SplitCsvLine(line);
            if (fields.Count < 9) continue;

            // Skip both English and Ukrainian header rows
            var col0 = fields[0].Trim();
            if (col0 is "ProductCode" or "Код номерклатури") continue;

            var itemId = fields[4].Trim();
            if (string.IsNullOrEmpty(itemId)) continue;

            // Price: may use comma as decimal separator (e.g. "17311,76") or dot
            var priceNormalized = fields[2].Trim().Replace(",", ".");
            if (!double.TryParse(priceNormalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
                continue;

            if (!int.TryParse(fields[3].Trim(), out var year)) continue;
            if (!int.TryParse(fields[6].Trim(), out var invoiceId)) continue;
            if (!DateOnly.TryParse(fields[7].Trim(), CultureInfo.InvariantCulture, out var invoiceDate)) continue;

            rows.Add(new CsvRow(
                ProductCode: col0,
                Name: fields[1].Trim(),
                Price: price,
                Year: year,
                ItemId: itemId,
                FactoryId: fields[5].Trim(),
                InvoiceId: invoiceId,
                InvoiceDate: invoiceDate,
                CategoryId: MapCategoryId(fields[8].Trim())
            ));
        }

        return rows;
    }

    /// <summary>
    /// RFC-4180-compatible CSV line splitter that handles quoted fields and escaped quotes ("").
    /// </summary>
    private static List<string> SplitCsvLine(string line)
    {
        var fields = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    // Escaped quote inside a quoted field
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++;
                    }
                    else inQuotes = false;
                }
                else sb.Append(c);
            }
            else
            {
                if (c == '"') inQuotes = true;
                else if (c == ',')
                {
                    fields.Add(sb.ToString());
                    sb.Clear();
                }
                else sb.Append(c);
            }
        }

        fields.Add(sb.ToString());
        return fields;
    }

    private static string? MapCategoryId(string raw) =>
        raw.ToLowerInvariant() switch
        {
            "арм" => "Computer",
            "оберіг" => "Oberig",
            "мережа" => "Network",
            "принтер" => "Printer",
            "радіозв'язок" => "Radio",
            "телефон" => "Phone",
            "бодіками" => "Video",
            _ => null // дбж, планшет, empty, etc.
        };
}