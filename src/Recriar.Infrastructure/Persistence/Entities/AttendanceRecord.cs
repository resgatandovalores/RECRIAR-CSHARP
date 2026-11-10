namespace Recriar.Infrastructure.Persistence.Entities;

public sealed class AttendanceRecord
{
    public int Id { get; set; }
    public int SessionEnrollmentId { get; set; }
    public bool Attended { get; set; }
    public DateTime RecordedAt { get; set; }
}
