using Microsoft.EntityFrameworkCore;
using Recriar.Application.Interfaces;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Context;

namespace Recriar.Infrastructure.Persistence.Repositories;

public sealed class ChildRepository(AppDbContext context) : IChildRepository
{
    public Child? GetById(int id) => context.Children
        .Include(x => x.Guardian)
        .Include(x => x.EmergencyContact)
        .Include(x => x.FamilyMembers).ThenInclude(x => x.RelationshipType)
        .SingleOrDefault(x => x.Id == id);

    public List<Child> GetAll() => context.Children.AsNoTracking().OrderBy(x => x.Id).ToList();

    public List<Child> SearchByName(string name) => context.Children.AsNoTracking()
        .Where(x => x.Name.Contains(name)).OrderBy(x => x.Id).ToList();

    public void Add(Child child)
    {
        try
        {
            context.Children.Add(child);
            context.SaveChanges();
        }
        catch { context.ChangeTracker.Clear(); throw; }
    }

    public void Update(Child child)
    {
        if (context.Entry(child).State == EntityState.Detached)
            throw new InvalidOperationException("Carregue o cadastro no mesmo contexto antes de editar.");
        // Save the child loaded by GetById, including family members added or removed.
        try { context.SaveChanges(); }
        catch { context.ChangeTracker.Clear(); throw; }
    }

    public bool Delete(int id)
    {
        var child = context.Children.Find(id);
        if (child is null) return false;
        try
        {
            context.Children.Remove(child);
            context.SaveChanges();
            return true;
        }
        catch { context.ChangeTracker.Clear(); throw; }
    }
}
