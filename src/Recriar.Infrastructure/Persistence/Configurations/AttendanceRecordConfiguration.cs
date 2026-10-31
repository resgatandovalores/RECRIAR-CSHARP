using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recriar.Application.Validators;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Entities;

namespace Recriar.Infrastructure.Persistence.Configurations;

public sealed class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("AttendanceRecords");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => x.SessionEnrollmentId).IsUnique();
        builder.HasOne<SessionEnrollment>().WithOne().HasForeignKey<AttendanceRecord>(x => x.SessionEnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
