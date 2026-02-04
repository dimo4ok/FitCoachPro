using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Services.Access;

public class ExerciseAccessService(
    IExerciseRepository exerciseRepository) : IExerciseAccessService
{
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    public bool HasUserAccess(UserRole currentRole) =>
        currentRole switch
        {
            UserRole.Coach => true,
            UserRole.Admin => true,
            _ => false
        };

    public async Task<bool> CanModifyExerciseAsync(Guid exerciseId, UserRole currentRole, CancellationToken cancellationToken) =>
        currentRole switch
        {
            UserRole.Coach => !await _exerciseRepository.IsExerciseUsedInActiveWorkoutPlanAsync(exerciseId, cancellationToken),
            UserRole.Admin => true,
            _ => false
        };
}
