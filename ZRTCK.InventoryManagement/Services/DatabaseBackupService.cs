using System.Data;
using Microsoft.EntityFrameworkCore;
using ZRTCK.InventoryManagement.Persistence;

namespace ZRTCK.InventoryManagement.Services;

public partial class DatabaseBackupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseBackupService> _logger;
    private readonly string _backupFolder;

    public DatabaseBackupService(IServiceProvider serviceProvider, ILogger<DatabaseBackupService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var backupPath = configuration.GetValue<string>("BackupSettings:Path") ?? "backups";
        if (!Path.IsPathRooted(backupPath))
        {
            backupPath = Path.Combine(AppContext.BaseDirectory, backupPath);
        }

        _backupFolder = backupPath;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Database Backup Service is starting.");
        Console.WriteLine("Database Backup Service is starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformBackupAsync();
                CleanOldBackups();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during database backup.");
            }

            // Wait for 1 hour
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task PerformBackupAsync()
    {
        Console.WriteLine("Performing database backup...");
        if (!Directory.Exists(_backupFolder))
        {
            Directory.CreateDirectory(_backupFolder);
        }

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
        var backupFileName = $"ZRTCKInventoryManagement_{timestamp}.db";
        var backupPath = Path.Combine(_backupFolder, backupFileName);

        LogCreatingDatabaseBackupBackuppath(backupPath);

        // For SQLite, a simple file copy is often enough if the DB is not heavily written to,
        // but it's safer to use VACUUM INTO or a proper backup API if we wanted to be 100% safe.
        // However, for this requirement, a file copy is usually what's expected unless specified otherwise.
        // We can use the SQLite Connection to perform a backup to be safe.

        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<InventoryDbContext>>();
            await using var context = await dbContextFactory.CreateDbContextAsync();
            var connection = context.Database.GetDbConnection();

            // Ensure connection is open
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using (var command = connection.CreateCommand())
            {
                command.CommandText = $"VACUUM INTO '{backupPath}'";
                await command.ExecuteNonQueryAsync();
            }
        }

        _logger.LogInformation("Database backup created successfully.");
    }

    private void CleanOldBackups()
    {
        _logger.LogInformation("Cleaning old backups...");

        if (!Directory.Exists(_backupFolder)) return;

        var backups = Directory.GetFiles(_backupFolder, "ZRTCKInventoryManagement_*.db")
            .Select(f => new { Path = f, Time = File.GetCreationTime(f) })
            .OrderByDescending(f => f.Time)
            .ToList();

        var now = DateTime.Now;

        // Retention strategy:
        // * store 24 hourly backups.
        // * store 30 daily backups.
        // * store all weekly backups.

        // Group backups
        var hourlyBackups = backups.Take(24).ToList();
        var olderBackups = backups.Skip(24).ToList();

        // From older backups, keep one per day for the last 30 days
        var dailyKeep = olderBackups
            .Where(b => b.Time >= now.AddDays(-30))
            .GroupBy(b => b.Time.Date)
            .Select(g => g.First())
            .ToList();

        // From even older or remaining, keep one per week (Monday as start of week)
        var weeklyKeep = backups
            .GroupBy(b => GetStartOfWeek(b.Time))
            .Select(g => g.First())
            .ToList();

        var keepPaths = new HashSet<string>(hourlyBackups.Select(b => b.Path));
        foreach (var b in dailyKeep) keepPaths.Add(b.Path);
        foreach (var b in weeklyKeep) keepPaths.Add(b.Path);

        foreach (var backup in backups)
        {
            if (keepPaths.Contains(backup.Path))
            {
                continue;
            }

            try
            {
                File.Delete(backup.Path);
                LogDeletedOldBackupPath(backup.Path);
            }
            catch (Exception ex)
            {
                LogFailedToDeleteOldBackupPath(backup.Path, ex);
            }
        }
    }

    private DateTime GetStartOfWeek(DateTime dt)
    {
        int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }

    [LoggerMessage(LogLevel.Information, "Deleted old backup: {Path}")]
    partial void LogDeletedOldBackupPath(string path);

    [LoggerMessage(LogLevel.Error, "Failed to delete old backup: {Path}")]
    partial void LogFailedToDeleteOldBackupPath(string path, Exception exception);

    [LoggerMessage(LogLevel.Information, "Creating database backup: {BackupPath}")]
    partial void LogCreatingDatabaseBackupBackuppath(string backupPath);
}