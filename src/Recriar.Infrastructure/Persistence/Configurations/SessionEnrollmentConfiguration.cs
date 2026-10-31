using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recriar.Application.Validators;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Entities;

namespace Recriar.Infrastructure.Persistence.Configurations;

public sealed class SessionEnrollmentConfiguration : IEntityTypeConfiguration<SessionEnrollment>
{
    public void Configure(EntityTypeBuilder<SessionEnrollment> builder)
    {
        builder.ToTable("SessionEnrollments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasIndex(x => new { x.SessionId, x.ChildId }).IsUnique();
        builder.HasOne<Child>().WithMany().HasForeignKey(x => x.ChildId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ActivitySession>().WithMany().HasForeignKey(x => x.SessionId).OnDelete(DeleteBehavior.Restrict);

    }
}
