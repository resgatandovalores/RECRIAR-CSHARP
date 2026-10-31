using Microsoft.EntityFrameworkCore;
using Recriar.Application.Interfaces;
using Recriar.Domain.Entities;
using Recriar.Infrastructure.Persistence.Context;

namespace Recriar.Infrastructure.Persistence.Repositories;

public class ChildRepository : IChildRepository
{
    private readonly AppDbContext _context;

    public ChildRepository(AppDbContext context)
    {
        _context = context;
    }

    public Child? GetById(int id)
    {
        // Include also loads the records linked to the child.
        Child? child = _context.Children
            .Include(child => child.Guardian)
            .Include(child => child.EmergencyContact)
            .Include(child => child.FamilyMembers)
            .ThenInclude(member => member.RelationshipType)
            .SingleOrDefault(child => child.Id == id);

        return child;
    }

    public List<Child> GetAll()
    {
        // This list is for reading, so EF does not need to track changes.
        List<Child> children = _context.Children
            .AsNoTracking()
            .OrderBy(child => child.Id)
            .ToList();

        return children;
    }

    public List<Child> SearchByName(string name)
    {
        List<Child> children = _context.Children
            .AsNoTracking()
            .Where(child => child.Name.Contains(name))
            .OrderBy(child => child.Id)
            .ToList();

        return children;
    }

    public void Add(Child child)
    {
        try
        {
            _context.Children.Add(child);
            _context.SaveChanges();
        }
        catch
        {
            // Discard pending changes so the next operation can start again.
            _context.ChangeTracker.Clear();
            throw;
        }
    }

    public void Update(Child child)
    {
        if (_context.Entry(child).State == EntityState.Detached)
        {
            throw new InvalidOperationException("Carregue o cadastro no mesmo contexto antes de editar.");
        }
        // Save the child loaded by GetById, including family members added or removed.
        try
        {
            _context.SaveChanges();
        }
        catch
        {
            _context.ChangeTracker.Clear();
            throw;
        }
    }

    public bool Delete(int id)
    {
        Child? child = _context.Children.Find(id);
        if (child == null)
        {
            return false;
        }

        try
        {
            _context.Children.Remove(child);
            _context.SaveChanges();
            return true;
        }
        catch
        {
            _context.ChangeTracker.Clear();
            throw;
        }
    }
}
