using Microsoft.EntityFrameworkCore;
using Recriar.Application.Interfaces;
using Recriar.Infrastructure.Persistence.Context;
using Recriar.Infrastructure.Persistence.Entities;

namespace Recriar.Infrastructure.Persistence.Repositories;

public class DbActivityLogger : IActivityLogger
{
    private readonly DbContextOptions<AppDbContext>? _options;

    public DbActivityLogger()
    {
    }

    // Tests can supply the settings for their temporary database.
    public DbActivityLogger(DbContextOptions<AppDbContext> options)
    {
        _options = options;
    }

    public void Log(string action, string description)
    {
        // Use a separate context so a log error does not affect the next registration.
        AppDbContext context;
        if (_options == null)
        {
            context = AppDbContextFactory.Create();
        }
        else
        {
            context = new AppDbContext(_options);
        }

        using (context)
        {
            ActivityLog log = new ActivityLog();
            log.Action = action;
            log.Description = description;
            log.CreatedAt = DateTime.UtcNow;

            context.ActivityLogs.Add(log);
            context.SaveChanges();
        }
    }
}
