using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recriar.Application.Validators;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Entities;

namespace Recriar.Infrastructure.Persistence.Configurations;

public sealed class FamilyMemberConfiguration : IEntityTypeConfiguration<FamilyMember>
{
    public void Configure(EntityTypeBuilder<FamilyMember> builder)
    {
        builder.ToTable("FamilyMembers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(FieldLimits.PersonName).IsRequired();
        builder.Property(x => x.OtherRelationship).HasMaxLength(FieldLimits.Relationship).IsRequired(false);
        builder.HasOne(x => x.RelationshipType).WithMany().HasForeignKey(x => x.RelationshipTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.ToTable(t => t.HasCheckConstraint("CK_FamilyMembers_Age", "`Age` BETWEEN 0 AND 120"));

    }
}
