using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Common.Models;

public class AuthModel
{
    public string Token { get; init; } = null!;
    public DateTime Expires { get; init; }
    public Guid Id { get; init; }
    public string UserName { get; init; } = null!;
    public UserRole Role { get; init; }
}
