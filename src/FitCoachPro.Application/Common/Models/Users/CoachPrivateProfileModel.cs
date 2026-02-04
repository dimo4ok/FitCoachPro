using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Common.Models.Users;

public record CoachPrivateProfileModel(
    Guid Id,
    string UserName,
    string Email,
    string? PhoneNumber,
    string FirstName,
    string LastName,
    DateTime CreatedAt,
    ClientAcceptanceStatus AcceptanceStatus);
