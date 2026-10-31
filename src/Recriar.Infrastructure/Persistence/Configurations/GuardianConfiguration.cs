using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recriar.Application.Validators;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Entities;

namespace Recriar.Infrastructure.Persistence.Configurations;

public sealed class GuardianConfiguration : IEntityTypeConfiguration<Guardian>
{
    public void Configure(EntityTypeBuilder<Guardian> builder)
    {
        builder.ToTable("Guardians");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(FieldLimits.PersonName).IsRequired();
        builder.Property(x => x.Cpf).HasMaxLength(FieldLimits.Document).IsRequired();
        builder.Property(x => x.Rg).HasMaxLength(FieldLimits.Document).IsRequired();
        builder.HasIndex(x => x.ChildId).IsUnique();
    }
}
