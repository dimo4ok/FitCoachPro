using FitCoachPro.Domain.Entities.Workouts;

namespace FitCoachPro.Application.Interfaces.Repository;

public interface IExerciseRepository
{
    Task<IReadOnlyList<Exercise>> GetAllAsync(CancellationToken cancellationToken = default);
}
