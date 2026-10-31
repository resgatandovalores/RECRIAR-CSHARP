using Microsoft.EntityFrameworkCore;
using Recriar.Application.Interfaces;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Context;

namespace Recriar.Infrastructure.Persistence.Repositories;

public class RelationshipTypeRepository : IRelationshipTypeRepository
{
    private readonly AppDbContext _context;

    public RelationshipTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<RelationshipType> GetAll()
    {
        List<RelationshipType> types = _context.RelationshipTypes
            .AsNoTracking()
            .OrderBy(type => type.Id)
            .ToList();

        return types;
    }
}
