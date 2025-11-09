using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Common.Models.Auth;

public class JwtPayloadModel
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = null!;
    public UserRole Role { get; init; }
}
