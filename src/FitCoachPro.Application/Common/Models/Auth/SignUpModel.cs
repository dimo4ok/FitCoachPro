using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Common.Models.Auth;

public class SignUpModel
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public UserRole Role { get; init; }

    public string Email { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public string Password { get; set; } = null!;
}
