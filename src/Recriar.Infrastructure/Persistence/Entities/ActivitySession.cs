namespace Recriar.Infrastructure.Persistence.Entities;

public sealed class ActivitySession
{
    public int Id { get; set; }
    public int ActivityId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string Location { get; set; } = string.Empty;
}
