using Microsoft.EntityFrameworkCore;

namespace ZRTCK.InventoryManagement.Persistence;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<InventoryItem> Inventory { get; set; }
    public DbSet<InventoryInvoice> Invoices { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<InventoryCategory> Categories { get; set; }
    public DbSet<InventoryCategoryField> CategoryFields { get; set; }
    public DbSet<InventoryAssignment> Assignments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
}