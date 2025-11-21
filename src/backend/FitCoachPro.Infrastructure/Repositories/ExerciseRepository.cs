using FitCoachPro.Application.Interfaces.Repository;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Infrastructure.Repositories;

public class ExerciseRepository(AppDbContext context) : IExerciseRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IReadOnlyList<Exercise>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var exerciseList = await _context.Exercises
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return exerciseList.AsReadOnly();
    }
}
