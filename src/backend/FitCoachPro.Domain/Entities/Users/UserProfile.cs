
using FitCoachPro.Domain.Entities.Identity;

namespace FitCoachPro.Domain.Entities.Users;

public abstract class UserProfile
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public byte[] RowVersion { get; set; } = null!;
}
