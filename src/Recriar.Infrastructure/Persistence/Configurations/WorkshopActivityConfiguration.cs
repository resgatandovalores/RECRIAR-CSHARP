using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recriar.Application.Validators;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Entities;

namespace Recriar.Infrastructure.Persistence.Configurations;

public sealed class WorkshopActivityConfiguration : IEntityTypeConfiguration<WorkshopActivity>
{
    public void Configure(EntityTypeBuilder<WorkshopActivity> builder)
    {
        builder.ToTable("Activities");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.HasOne<WorkshopProgram>().WithMany().HasForeignKey(x => x.ProgramId).OnDelete(DeleteBehavior.Restrict);

    }
}
