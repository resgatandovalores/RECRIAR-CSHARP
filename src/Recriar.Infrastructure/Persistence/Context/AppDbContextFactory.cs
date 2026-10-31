using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MySqlConnector;

namespace Recriar.Infrastructure.Persistence.Context;

public static class AppDbContextFactory
{
    public const string ConnectionVariable = "RECRIAR_CONNECTION_STRING";

    public static AppDbContext Create()
    {
        string? value = Environment.GetEnvironmentVariable(ConnectionVariable);
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Configure a variável de ambiente {ConnectionVariable} na sua sessão.");

        MySqlConnectionStringBuilder connection;
        try { connection = new MySqlConnectionStringBuilder(value); }
        catch (ArgumentException)
        {
            throw new InvalidOperationException("A configuração local de conexão é inválida.");
        }

        // This development factory must never target a shared or production server.
        if (connection.Database != "recriar_dev" ||
            !(connection.Server.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
              connection.Server == "127.0.0.1" || connection.Server == "::1"))
            throw new InvalidOperationException("Use o banco recriar_dev em um servidor local para desenvolvimento.");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(value, new MySqlServerVersion(new Version(8, 0, 16)))
            .Options;
        return new AppDbContext(options);
    }
}

public sealed class DesignTimeAppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args) => AppDbContextFactory.Create();
}
