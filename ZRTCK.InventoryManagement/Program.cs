using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using ZRTCK.InventoryManagement;
using ZRTCK.InventoryManagement.Components;
using ZRTCK.InventoryManagement.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddDbContextFactory<InventoryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

var supportedCultures = new[] { "uk-UA" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// Ensure database is created on startup; seed from CSV if it was just created
if (args.Length > 0)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<InventoryDbContext>>();
        await using var context = await dbContextFactory.CreateDbContextAsync();

        if (args[0] == "migrate")
        {
            Console.WriteLine("Applying migrations...");
            await context.Database.MigrateAsync();
            Console.WriteLine("Migrations applied.");
            return;
        }

        if (args[0] == "load" && args.Length > 1)
        {
            var fileName = args[1];
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"File not found: {fileName}");
                return;
            }
            await CsvInventoryLoader.LoadFromCsvAsync(context, fileName);
            return;
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();