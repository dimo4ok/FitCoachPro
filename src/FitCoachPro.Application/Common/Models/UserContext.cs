using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Common.Models;

public record UserContext(Guid UserId, UserRole Role);
