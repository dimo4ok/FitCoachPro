using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Interfaces.Services.Access;

public interface IExerciseAccessService
{
    Task<bool> CanModifyExerciseAsync(Guid exerciseId, UserRole currentRole, CancellationToken cancellationToken);
    bool HasUserAccess(UserRole currentRole);
}