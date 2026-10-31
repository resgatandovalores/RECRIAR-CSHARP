using Recriar.Application.Interfaces;
using Recriar.Infrastructure.Persistence.Context;
using Recriar.Infrastructure.Persistence.Entities;

namespace Recriar.Infrastructure.Persistence.Repositories;

public sealed class DbActivityLogger : IActivityLogger
{
    private readonly Func<AppDbContext> _createContext;

    public DbActivityLogger() : this(AppDbContextFactory.Create) { }

    public DbActivityLogger(Func<AppDbContext> createContext) => _createContext = createContext;

    public void Log(string action, string description)
    {
        // A failed log must not leave pending changes in the repositories' context.
        using var context = _createContext();
        context.ActivityLogs.Add(new ActivityLog
        {
            Action = action,
            Description = description,
            CreatedAt = DateTime.UtcNow
        });
        context.SaveChanges();
    }
}
