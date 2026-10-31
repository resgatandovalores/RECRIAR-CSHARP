using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recriar.Application.Validators;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Entities;

namespace Recriar.Infrastructure.Persistence.Configurations;

public sealed class RelationshipTypeConfiguration : IEntityTypeConfiguration<RelationshipType>
{
    public void Configure(EntityTypeBuilder<RelationshipType> builder)
    {
        builder.ToTable("RelationshipTypes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Name).HasMaxLength(FieldLimits.Relationship).IsRequired();
        builder.HasData(
            new RelationshipType { Id = 1, Name = "Mãe" },
            new RelationshipType { Id = 2, Name = "Pai" },
            new RelationshipType { Id = 3, Name = "Avó" },
            new RelationshipType { Id = 4, Name = "Avô" },
            new RelationshipType { Id = 5, Name = "Irmã" },
            new RelationshipType { Id = 6, Name = "Irmão" },
            new RelationshipType { Id = 7, Name = "Tia" },
            new RelationshipType { Id = 8, Name = "Tio" },
            new RelationshipType { Id = RelationshipType.OtherId, Name = "Outro" });
    }
}
