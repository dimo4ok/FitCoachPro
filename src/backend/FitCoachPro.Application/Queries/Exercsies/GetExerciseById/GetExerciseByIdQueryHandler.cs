using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models.Workouts.Exercise;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Queries.Exercsies.GetExerciseById;

public class GetExerciseByIdQueryHandler(
    IUserContextService userContext,
    IExerciseRepository exerciseRepository,
    IExerciseAccessService accessService
    ) : IQueryHandler<GetExerciseByIdQuery, Result<ExerciseDetailModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IExerciseAccessService _accessService = accessService;

    public async Task<Result<ExerciseDetailModel>> ExecuteAsync(GetExerciseByIdQuery query, CancellationToken cancellationToken)
    {
        if (!_accessService.HasUserAccess(_userContext.Current.Role))
            return Result<ExerciseDetailModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var exercise = await _exerciseRepository.GetByIdAsync(query.Id, cancellationToken);
        if (exercise == null)
            return Result<ExerciseDetailModel>.Fail(DomainErrors.NotFound(nameof(Exercise)));

        return Result<ExerciseDetailModel>.Success(exercise.ToModel());
    }
}