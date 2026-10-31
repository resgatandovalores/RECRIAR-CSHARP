using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recriar.Application.Validators;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Entities;

namespace Recriar.Infrastructure.Persistence.Configurations;

public sealed class ActivitySessionConfiguration : IEntityTypeConfiguration<ActivitySession>
{
    public void Configure(EntityTypeBuilder<ActivitySession> builder)
    {
        builder.ToTable("ActivitySessions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Location).HasMaxLength(100).IsRequired();
        builder.HasOne<WorkshopActivity>().WithMany().HasForeignKey(x => x.ActivityId).OnDelete(DeleteBehavior.Restrict);
        builder.ToTable(t => t.HasCheckConstraint("CK_ActivitySessions_Period", "`EndsAt` > `StartsAt`"));
        builder.HasIndex(x => x.StartsAt);

    }
}
