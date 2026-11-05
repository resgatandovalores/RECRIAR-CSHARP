using MySqlConnector;

namespace Recriar.Infrastructure.Persistence.Context;

// Keeps the connection settings. Each operation opens and closes its own connection.
// The name was kept so the screens can use the constructors agreed by the team.
public class AppDbContext : IDisposable
{
    private readonly string _writeConnectionString;
    private readonly string _readConnectionString;

    public AppDbContext(string writeConnectionString, string readConnectionString)
    {
        _writeConnectionString = writeConnectionString;
        _readConnectionString = readConnectionString;
    }

    public MySqlConnection CreateWriteConnection()
    {
        return new MySqlConnection(_writeConnectionString);
    }

    public MySqlConnection CreateReadConnection()
    {
        return new MySqlConnection(_readConnectionString);
    }

    public void Dispose()
    {
        // Connections are disposed by the methods that use them.
    }
}
