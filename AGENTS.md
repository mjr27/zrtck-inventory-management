# AGENTS.md - ZRTCK Inventory Management System

## Project Overview

**Tech Stack:** .NET 10.0, Blazor (Interactive Server), Entity Framework Core 10, SQLite, MudBlazor UI

This is a Ukrainian-language inventory management system for tracking IT equipment (computers, printers, networking gear, etc.) and managing their assignment to personnel through official orders.

**Key Domain Concepts:**
- **Invoices** (`InventoryInvoice`): Bulk imports of equipment with purchase dates
- **Items** (`InventoryItem`): Individual pieces of equipment (UUID-based IDs), typed via `CategoryId` linking to `InventoryCategory` table.
- **Assignments** (`InventoryAssignment`): Official orders assigning items to people
- **Assignment Rows** (`InventoryAssignmentRow`): Junction table linking items to people within an assignment

---

## Architecture & Data Flow

### Database Layer (Persistence/)
- **Schema**: All tables use snake_case conventions via `EFCore.NamingConventions`
- **Configuration-based**: Entity configs in `Persistence/Configuration/` apply relationships and type mappings
- **Key Pattern**: `InventoryCategory` is used for equipment types.
- **DbContext Factory**: Used throughout UI for transient context creation (not singleton)

**Entity Relationships:**
```
InventoryInvoice (1) ──→ (Many) InventoryItem
InventoryItem (Many) ──→ (0..1) InventoryCategory
InventoryAssignment (1) ──→ (Many) InventoryAssignmentRow (Many) ──→ (1) Person
InventoryAssignmentRow ──→ InventoryItem (many items per assignment)
```

### Bootstrap Process (Program.cs)
1. On first app startup, `Database.EnsureCreatedAsync()` creates schema
2. If DB was just created, `CsvInventoryLoader.LoadFromCsvAsync()` seeds from `maino.csv`
3. CSV path search walks up directory tree from AppContext.BaseDirectory

### CSV Import Pipeline (CsvInventoryLoader.cs)
- **Format**: RFC-4180 CSV with Ukrainian headers or English equivalents
- **Flow**: 
  - Parse → Deduplicate by ItemId → Group by InvoiceId → Create invoices with items
- **Quirks**: 
  - Handles comma/dot decimal separators (normalizes to dot)
  - CSV contains intentional duplicates at end (deduped by `First()`)
  - Runs only on fresh DB creation; won't re-seed existing data

---

## UI Architecture (Blazor)

### Rendering Mode
- **Default**: `@rendermode InteractiveServer` (stateful server-side rendering)
- **QuickGrid commented out** (originally considered, replaced with MudBlazor)
- **MudBlazor** handles data grids, dialogs, snackbars, and dropdowns

### Pages (Components/Pages/)
- **Inventory.razor** (`/`): Browse all items with client-side search + ProductType filter
- **Invoices.razor**: View invoice summaries
- **Assignments.razor** (`/assignments`): CRUD for InventoryAssignment orders
- **CreateAssignment.razor** / **EditAssignment.razor**: Forms with nested AssignmentRowsEditor
- **People.razor**: Manage personnel records

### Reusable Components
- **AssignmentRowsEditor.razor**: Shared component that:
  - Displays all inventory items in a MudTable
  - Two-way binds selected people via `Dictionary<ItemId, PersonId?>`
  - Parent passes data via `[Parameter]` and receives updates via `EventCallback`

### Filtering Patterns
- **In-memory client-side** on loaded data (no backend filtering ATM)
- Example: Inventory.razor uses LINQ Where() on loaded items for search + ProductType

---

## Developer Workflows

### Build & Run
```bash
# From workspace root
dotnet build
dotnet run --project ZRTCK.InventoryManagement/ZRTCK.InventoryManagement.csproj
# App runs on https://localhost:5001 (check launchSettings.json)
```

### Database Reset
- Delete `ZRTCKInventoryManagement.db*` files (db, db-shm, db-wal)
- Next app start: schema auto-created + CSV auto-seeded

### Adding Entities
1. Create class in `Persistence/` namespace
2. Add `DbSet<T>` to `InventoryDbContext`
3. Create `IEntityTypeConfiguration<T>` in `Configuration/` subfolder
4. Relationships & conversions go in the config class (not entity decorators)

### EF Core Patterns Used
- **Fluent API** (Configuration classes) for relationships and type mappings
- **shadow properties** implicitly handled by naming convention
- **DbContextFactory<T>** for scope management (each Razor component creates own context)

---

## Project-Specific Conventions

### Naming & Localization
- UI labels are primarily **Ukrainian**
- C# code uses English identifiers
- CSV can have English or Ukrainian headers (both handled)

### Inventory Categories
Equipment types are managed via the `InventoryCategory` table:
- Computer (Комп'ютер)
- Printer (Принтер)
- Network (Мережа)
- Phone (Телефон)
- Radio (Радіо)
- Oberig (Оберіг)
- Video (Відео)

Mapping logic in `CsvInventoryLoader.MapCategoryId()` handles unmapped types by returning null.

### Item IDs
- **InventoryItem.Id**: String (max 36 chars), often UUID-like or inventory codes
- **InventoryInvoice.Id**: Actually an int (despite StringLength annotation - likely a schema bug)
- **Person.Id, InventoryAssignment.Id**: Auto-incrementing ints

### Data Initialization
- **Fixtures.cs**: Hardcoded test invoices + items (used in development, not auto-loaded)
- Commented-out large dataset at end; uncomment if static seeding needed

---

## Key Dependencies

### NuGet Packages
- `Microsoft.EntityFrameworkCore.Sqlite` 10.0.5
- `EFCore.NamingConventions` 10.0.1 (snake_case in DB)
- `MudBlazor` 9.3.0 (UI components)
- `Enums.NET` 5.0.0 (ProductType display/parsing utilities)

### Bootstrap Files
- `appsettings.json`: SQLite connection string (relative path `./ZRTCKInventoryManagement.db`)
- `launchSettings.json`: HTTPS URLs and environment variables

---

## Testing & Debugging

### CSV Issues
- If import fails silently, check console output (CsvInventoryLoader logs to Console)
- Verify `maino.csv` is in solution root or next to binary
- Check for extra spaces, BOM, or encoding issues in CSV

### Component State Issues
- Blazor components are **stateful per connection** (InteractiveServer)
- Disconnects/reconnects may lose UI state; ReconnectModal.razor handles reconnection flow

### EF Core Query Issues
- Remember to `.Include()` related entities (e.g., `InventoryItem.InventoryInvoice`)
- Queries run against **in-memory DbSet** after `.ToListAsync()` (no lazy loading with InteractiveServer)

---

## Common Tasks for AI

### Adding a New Filter to Inventory Page
→ Modify client-side Where() clause in Inventory.razor `@code` block; optionally bind to new `<MudSelect>` for UI control

### Creating Assignment from Template
→ Edit Assignments.razor or CreateAssignment.razor to pre-populate rows; use AssignmentRowsEditor with default selections

### Modifying CSV Schema
→ Update `CsvRow` record in CsvInventoryLoader, adjust field indices in `ParseCsv()`, and update Category mapping if needed

### Adding Persistence Fields
→ Add property to entity class, create/update Configuration using Fluent API, verify naming convention compliance

---

## Notes for Maintainers

- **Ukranian labels**: Do not translate; they are part of business requirements
- **EFCore.NamingConventions**: Critical for DB compatibility; changing table names requires migration strategy
- **IDbContextFactory**: Always use this pattern; singleton DbContext is not thread-safe in server-side Blazor
- **CSV import**: One-time operation on fresh DB; subsequent edits must use UI (no idempotent re-import)

