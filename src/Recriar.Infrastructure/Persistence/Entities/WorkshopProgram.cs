namespace Recriar.Infrastructure.Persistence.Entities;

public sealed class WorkshopProgram
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
