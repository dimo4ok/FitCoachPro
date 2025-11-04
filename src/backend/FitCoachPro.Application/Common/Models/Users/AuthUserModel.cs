using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Common.Models.Users;

public class AuthUserModel
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public UserRole Role { get; init; }
}
