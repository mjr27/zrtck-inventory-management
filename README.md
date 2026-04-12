# ZRTCK Inventory Management

Inventory management system for tracking IT equipment, invoices, and assignment orders.

## Tech stack

- .NET 10 (`net10.0`)
- Blazor (Interactive Server)
- Entity Framework Core 10 + SQLite
- MudBlazor UI
- `EFCore.NamingConventions` (snake_case database naming)

## Project structure

- `ZRTCK.InventoryManagement/` - main web application
- `ZRTCK.InventoryManagement/Persistence/` - entities, `InventoryDbContext`, EF configuration
- `ZRTCK.InventoryManagement/Components/Pages/` - page components
- `ZRTCK.InventoryManagement/Components/Controls/` - reusable UI controls
- `maino.csv` - bootstrap dataset used on first database creation
- `AGENTS.md` - project-specific engineering notes and conventions

## Quick start

### Prerequisites

- .NET SDK 10.0+

### Build

```bash
dotnet build /home/mjr/РТЦК/ZRTCK.InventoryManagement/ZRTCK.InventoryManagement.slnx
```

### Run

```bash
dotnet run --project /home/mjr/РТЦК/ZRTCK.InventoryManagement/ZRTCK.InventoryManagement/ZRTCK.InventoryManagement.csproj
```

Default development URL from `launchSettings.json`:

- `http://localhost:5135`

## Data bootstrap behavior

On application startup (`Program.cs`):

1. `Database.EnsureCreatedAsync()` initializes the schema if the DB does not exist.
2. If the DB is newly created, `CsvInventoryLoader.LoadFromCsvAsync()` imports data from `maino.csv`.

Notes:

- CSV import is a one-time bootstrap path for a fresh database.
- Existing databases are not automatically re-seeded.
- Connection string defaults to `Data Source=./ZRTCKInventoryManagement.db` (`appsettings.json`).

## Database reset (development)

Remove SQLite files near the app project, then start the app again.

```bash
rm -f /home/mjr/РТЦК/ZRTCK.InventoryManagement/ZRTCK.InventoryManagement/ZRTCKInventoryManagement.db*
```

## Core domain entities

- `InventoryInvoice` - invoice-level import group
- `InventoryItem` - individual equipment item
- `InventoryCategory` - item category/type lookup
- `InventoryAssignment` - assignment order header
- `InventoryAssignmentRow` - assignment order rows
- `Person` - personnel records

## Notes for contributors

- Keep UI labels and business-facing text in Ukrainian where required by domain needs.
- Use `IDbContextFactory<InventoryDbContext>` in components/services (do not use a singleton DbContext).
- Put relationship and mapping rules in `Persistence/Configuration/` (Fluent API).

For more implementation details and workflows, see `AGENTS.md`.

