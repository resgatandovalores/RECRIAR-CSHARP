using Recriar.Domain.Entities;

namespace Recriar.Application.Interfaces;

/// <summary>
/// Contract for reading the kinship options (Mãe, Pai, Avó...) from the database.
/// Implemented by the database project (Recriar.Infrastructure/Persistence/Repositories).
/// </summary>
public interface IRelationshipTypeRepository
{
    /// <summary>
    /// Returns every kinship option, ordered by Id.
    /// The table is filled by the seed, so it already has the 9 initial options.
    /// </summary>
    List<RelationshipType> GetAll();
}
