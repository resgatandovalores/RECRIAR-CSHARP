namespace Recriar.Infrastructure.Persistence.Entities;

public sealed class WorkshopActivity
{
    public int Id { get; set; }
    public int ProgramId { get; set; }
    public string Name { get; set; } = string.Empty;
}
