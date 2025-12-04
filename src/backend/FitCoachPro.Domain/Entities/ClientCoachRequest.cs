using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;

namespace FitCoachPro.Domain.Entities;

public class ClientCoachRequest
{
    public Guid Id { get; set; }
    public CoachRequestStatus Status { get; set; } = CoachRequestStatus.Pending;
    public string? Comment { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }

    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public Guid CoachId { get; set; }
    public Coach Coach { get; set; } = null!;

}
