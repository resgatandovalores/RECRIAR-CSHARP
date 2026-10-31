using Microsoft.EntityFrameworkCore;
using Recriar.Application.Interfaces;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Context;

namespace Recriar.Infrastructure.Persistence.Repositories;

public sealed class RelationshipTypeRepository(AppDbContext context) : IRelationshipTypeRepository
{
    public List<RelationshipType> GetAll() => context.RelationshipTypes.AsNoTracking().OrderBy(x => x.Id).ToList();
}
