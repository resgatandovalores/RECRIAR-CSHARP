using Microsoft.EntityFrameworkCore;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Entities;

namespace Recriar.Infrastructure.Persistence.Context;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Child> Children => Set<Child>();
    public DbSet<Guardian> Guardians => Set<Guardian>();
    public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();
    public DbSet<FamilyMember> FamilyMembers => Set<FamilyMember>();
    public DbSet<RelationshipType> RelationshipTypes => Set<RelationshipType>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<WorkshopProgram> Programs => Set<WorkshopProgram>();
    public DbSet<WorkshopActivity> Activities => Set<WorkshopActivity>();
    public DbSet<ActivitySession> ActivitySessions => Set<ActivitySession>();
    public DbSet<SessionEnrollment> SessionEnrollments => Set<SessionEnrollment>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasCharSet("utf8mb4").UseCollation("utf8mb4_0900_ai_ci");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
