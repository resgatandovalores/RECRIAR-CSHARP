using Microsoft.EntityFrameworkCore;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Entities;
using Recriar.Infrastructure.Persistence.Configurations;

namespace Recriar.Infrastructure.Persistence.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Child> Children
    {
        get
        {
            return Set<Child>();
        }
    }

    public DbSet<Guardian> Guardians
    {
        get
        {
            return Set<Guardian>();
        }
    }

    public DbSet<EmergencyContact> EmergencyContacts
    {
        get
        {
            return Set<EmergencyContact>();
        }
    }

    public DbSet<FamilyMember> FamilyMembers
    {
        get
        {
            return Set<FamilyMember>();
        }
    }

    public DbSet<RelationshipType> RelationshipTypes
    {
        get
        {
            return Set<RelationshipType>();
        }
    }

    public DbSet<ActivityLog> ActivityLogs
    {
        get
        {
            return Set<ActivityLog>();
        }
    }

    public DbSet<WorkshopProgram> Programs
    {
        get
        {
            return Set<WorkshopProgram>();
        }
    }

    public DbSet<WorkshopActivity> Activities
    {
        get
        {
            return Set<WorkshopActivity>();
        }
    }

    public DbSet<ActivitySession> ActivitySessions
    {
        get
        {
            return Set<ActivitySession>();
        }
    }

    public DbSet<SessionEnrollment> SessionEnrollments
    {
        get
        {
            return Set<SessionEnrollment>();
        }
    }

    public DbSet<AttendanceRecord> AttendanceRecords
    {
        get
        {
            return Set<AttendanceRecord>();
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasCharSet("utf8mb4").UseCollation("utf8mb4_0900_ai_ci");
        modelBuilder.ApplyConfiguration(new ChildConfiguration());
        modelBuilder.ApplyConfiguration(new GuardianConfiguration());
        modelBuilder.ApplyConfiguration(new EmergencyContactConfiguration());
        modelBuilder.ApplyConfiguration(new FamilyMemberConfiguration());
        modelBuilder.ApplyConfiguration(new RelationshipTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ActivityLogConfiguration());
        modelBuilder.ApplyConfiguration(new WorkshopProgramConfiguration());
        modelBuilder.ApplyConfiguration(new WorkshopActivityConfiguration());
        modelBuilder.ApplyConfiguration(new ActivitySessionConfiguration());
        modelBuilder.ApplyConfiguration(new SessionEnrollmentConfiguration());
        modelBuilder.ApplyConfiguration(new AttendanceRecordConfiguration());
    }
}
