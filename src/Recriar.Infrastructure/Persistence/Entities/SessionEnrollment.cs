namespace Recriar.Infrastructure.Persistence.Entities;

public sealed class SessionEnrollment
{
    public int Id { get; set; }
    public int ChildId { get; set; }
    public int SessionId { get; set; }
    public DateTime RegisteredAt { get; set; }
}
