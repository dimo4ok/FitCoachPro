using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Common.Models;

public record CurrentUserModel(Guid UserId, UserRole Role);
