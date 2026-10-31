using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recriar.Application.Validators;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Entities;

namespace Recriar.Infrastructure.Persistence.Configurations;

public sealed class EmergencyContactConfiguration : IEntityTypeConfiguration<EmergencyContact>
{
    public void Configure(EntityTypeBuilder<EmergencyContact> builder)
    {
        builder.ToTable("EmergencyContacts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(FieldLimits.PersonName).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(FieldLimits.Phone).IsRequired();
        builder.HasIndex(x => x.ChildId).IsUnique();
    }
}
