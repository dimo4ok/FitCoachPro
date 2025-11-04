using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Common.Models.Users;

public class JwtPayloadModel
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = null!;
    public UserRole Role { get; init; }
}
