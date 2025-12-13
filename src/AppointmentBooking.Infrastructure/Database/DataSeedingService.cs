using AppointmentBooking.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AppointmentBooking.Infrastructure.Database;

public class DataSeedingService
{
    private readonly EntityContext _context;
    private readonly IConfiguration _configuration;
    private readonly string _scriptsPath;

    public DataSeedingService(
        EntityContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;

        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        _scriptsPath = Path.Combine(basePath, "Scripts");
    }

    public async Task SeedDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Console.WriteLine("Starting data seeding process...");

            await EnsureMigrationHistoryTableExistsAsync(cancellationToken);

            if (!Directory.Exists(_scriptsPath))
            {
                Directory.CreateDirectory(_scriptsPath);
                return;
            }

            var scriptFiles = Directory.GetFiles(_scriptsPath, "*.sql")
                .OrderBy(f => f)
                .ToList();

            if (scriptFiles.Count == 0)
            {
                return;
            }


            var connectionString = _configuration.GetConnectionString("WriteConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("WriteConnection string is not configured");
            }

            foreach (var scriptFile in scriptFiles)
            {
                var scriptName = Path.GetFileName(scriptFile);

                var alreadyExecuted = await _context.DataMigrationHistory
                    .AnyAsync(h => h.ScriptName == scriptName, cancellationToken);

                if (alreadyExecuted)
                {
                    Console.WriteLine($"Script {scriptName} has already been executed. Skipping.");
                    continue;
                }

                Console.WriteLine($"Executing script: {scriptName}");

                try
                {
                    var scriptContent = await File.ReadAllTextAsync(scriptFile, cancellationToken);

                    await ExecuteSqlScriptAsync(connectionString, scriptContent, cancellationToken);

                    var history = new DataMigrationHistory
                    {
                        ScriptName = scriptName,
                        ExecutedAt = DateTime.UtcNow,
                        Description = $"Executed from {scriptFile}"
                    };

                    _context.DataMigrationHistory.Add(history);
                    await _context.SaveChangesAsync(cancellationToken);

                    Console.WriteLine($"Successfully executed and recorded script: {scriptName}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error executing script {scriptName}. Script will not be marked as executed.", ex); // Re-throw to stop the seeding process on error
                }
            }

        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while seeding the database", ex);
        }
    }

    private async Task EnsureMigrationHistoryTableExistsAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _context.DataMigrationHistory.FirstOrDefaultAsync(cancellationToken);
        }
        catch
        {
            throw new Exception("DataMigrationHistory table does not exist");
        }
    }

    private async Task ExecuteSqlScriptAsync(string connectionString, string script, CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(script, connection);
        command.CommandTimeout = 300;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
