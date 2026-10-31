using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recriar.Application.Validators;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Entities;

namespace Recriar.Infrastructure.Persistence.Configurations;

public sealed class ChildConfiguration : IEntityTypeConfiguration<Child>
{
    public void Configure(EntityTypeBuilder<Child> builder)
    {
        builder.ToTable("Children");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(FieldLimits.PersonName).IsRequired();
        builder.Property(x => x.Cpf).HasMaxLength(FieldLimits.Document).IsRequired();
        builder.Property(x => x.Rg).HasMaxLength(FieldLimits.Document).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(FieldLimits.Address).IsRequired();
        builder.Property(x => x.Neighborhood).HasMaxLength(FieldLimits.Neighborhood).IsRequired();
        builder.Property(x => x.City).HasMaxLength(FieldLimits.City).IsRequired();
        builder.Property(x => x.School).HasMaxLength(FieldLimits.School).IsRequired();
        builder.Property(x => x.MotherName).HasMaxLength(FieldLimits.PersonName).IsRequired();
        builder.Property(x => x.FatherName).HasMaxLength(FieldLimits.PersonName).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(FieldLimits.Phone).IsRequired();
        builder.Property(x => x.HealthCenter).HasMaxLength(FieldLimits.HealthCenter).IsRequired();
        builder.Property(x => x.FileNumber).HasMaxLength(FieldLimits.FileNumber).IsRequired();
        builder.Property(x => x.Allergy).HasMaxLength(FieldLimits.Allergy).IsRequired();
        builder.Property(x => x.RegistrationDate).HasColumnType("date");
        builder.HasIndex(x => x.Name);
        builder.ToTable(t => t.HasCheckConstraint("CK_Children_Age", "`Age` BETWEEN 0 AND 120"));
        builder.HasOne(x => x.Guardian).WithOne(x => x.Child)
            .HasForeignKey<Guardian>(x => x.ChildId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.EmergencyContact).WithOne(x => x.Child)
            .HasForeignKey<EmergencyContact>(x => x.ChildId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.FamilyMembers).WithOne(x => x.Child)
            .HasForeignKey(x => x.ChildId).IsRequired().OnDelete(DeleteBehavior.Cascade);
    }
}
