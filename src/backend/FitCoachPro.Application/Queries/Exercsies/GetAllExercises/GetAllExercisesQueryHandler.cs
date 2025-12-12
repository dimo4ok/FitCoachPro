using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Workouts.Exercise;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Application.Queries.Exercsies.GetAllExercises;

public class GetAllExercisesQueryHandler(
    IUserContextService userContext,
    IExerciseRepository exerciseRepository,
    IExerciseAccessService accessService
    ) : IQueryHandler<GetAllExercisesQuery, Result<PaginatedModel<ExerciseDetailModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IExerciseAccessService _accessService = accessService;

    public async Task<Result<PaginatedModel<ExerciseDetailModel>>> ExecuteAsync(GetAllExercisesQuery query, CancellationToken cancellationToken)
    {
        if (!_accessService.HasUserAccess(_userContext.Current.Role))
            return Result<PaginatedModel<ExerciseDetailModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var exercsiesQuery = _exerciseRepository.GetAllAsQuery();
        if (!await exercsiesQuery.AnyAsync(cancellationToken))
            return Result<PaginatedModel<ExerciseDetailModel>>.Fail(DomainErrors.NotFound(nameof(Exercise)));

        var paginated = await exercsiesQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize, cancellationToken);

        return Result<PaginatedModel<ExerciseDetailModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }
}