using MySqlConnector;
using Recriar.Application.Interfaces;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Context;

namespace Recriar.Infrastructure.Persistence.Repositories;

public class DbActivityLogger : IActivityLogger
{
    private readonly AppDbContext? _context;

    public DbActivityLogger()
    {
    }

    public DbActivityLogger(AppDbContext context)
    {
        _context = context;
    }

    public void Log(string action, string description)
    {
        AppDbContext context;
        if (_context == null)
        {
            context = AppDbContextFactory.Create();
        }
        else
        {
            context = _context;
        }

        // This connection is separate from the one that saved the child.
        using (MySqlConnection connection = context.CreateWriteConnection())
        {
            connection.Open();
            string sql = "INSERT INTO ActivityLogs (Action, Description, CreatedAt) " +
                "VALUES (@action, @description, @createdAt)";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@action", action);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
                command.ExecuteNonQuery();
            }
        }
    }
}
