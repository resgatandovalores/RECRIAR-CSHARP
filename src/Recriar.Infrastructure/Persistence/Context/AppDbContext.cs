using MySqlConnector;

namespace Recriar.Infrastructure.Persistence.Context;

// Stores the settings used to open the two connections.
public class AppDbContext
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
}
