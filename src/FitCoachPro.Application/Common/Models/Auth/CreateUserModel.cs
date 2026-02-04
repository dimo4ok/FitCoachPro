using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Common.Models.Auth;

public class CreateUserModel
{
    public Guid UserId { get; set; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public UserRole Role { get; init; }
}
