using DataProcessorService.Abstractions;
using DataProcessorService.Models;
using Microsoft.Data.Sqlite;

namespace DataProcessorService.Services;

public sealed class ModuleRepository : IModuleRepository
{
    private readonly string _connectionString;

    public ModuleRepository(string connectionString)
    {
        _connectionString = connectionString;

        CheckAvailability();

        SQLitePCL.Batteries.Init();

        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText =
        """
        CREATE TABLE IF NOT EXISTS Modules (
            ModuleCategoryID TEXT PRIMARY KEY,
            ModuleState TEXT NOT NULL
        );
        """;

        command.ExecuteNonQueryAsync();
    }

    private bool CheckAvailability()
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1;";
            command.ExecuteScalar();

            return true;
        }
        catch
        {
            throw;
        }
    }

    public async Task UpsertAsync(Module module, CancellationToken cancellationToken = default)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();
        command.CommandText =
        """
        INSERT INTO Modules (ModuleCategoryID, ModuleState)
        VALUES ($id, $state)
        ON CONFLICT(ModuleCategoryID) DO UPDATE SET ModuleState = $state;
        """;

        command.Parameters.AddWithValue("$id", module.ModuleCategoryId);
        command.Parameters.AddWithValue("$state", module.ModuleState);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
