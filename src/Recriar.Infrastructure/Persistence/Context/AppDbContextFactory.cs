using MySqlConnector;

namespace Recriar.Infrastructure.Persistence.Context;

public static class AppDbContextFactory
{
    public const string ConnectionVariable = "RECRIAR_CONNECTION_STRING";
    public const string ReadConnectionVariable = "RECRIAR_READ_CONNECTION_STRING";

    public static AppDbContext Create()
    {
        string? writeValue = Environment.GetEnvironmentVariable(ConnectionVariable);
        string? readValue = Environment.GetEnvironmentVariable(ReadConnectionVariable);
        if (string.IsNullOrWhiteSpace(writeValue) || string.IsNullOrWhiteSpace(readValue))
        {
            throw new InvalidOperationException("Configure as duas conexões: RECRIAR_CONNECTION_STRING e RECRIAR_READ_CONNECTION_STRING.");
        }

        MySqlConnectionStringBuilder write;
        MySqlConnectionStringBuilder read;
        try
        {
            write = new MySqlConnectionStringBuilder(writeValue);
            read = new MySqlConnectionStringBuilder(readValue);
        }
        catch (ArgumentException)
        {
            throw new InvalidOperationException("Confira os dados das conexões locais.");
        }

        if (!IsLocal(write.Server) || !IsLocal(read.Server) || write.Port != read.Port)
        {
            throw new InvalidOperationException("Use os dois bancos no mesmo servidor MySQL local.");
        }
        if (write.Database != "recriar_prod" || read.Database != "recriar_leitura")
        {
            throw new InvalidOperationException("Use recriar_prod para gravação e recriar_leitura para consultas.");
        }
        return new AppDbContext(writeValue, readValue);
    }

    private static bool IsLocal(string server)
    {
        return server.Equals("localhost", StringComparison.OrdinalIgnoreCase)
            || server == "127.0.0.1" || server == "::1";
    }
}
