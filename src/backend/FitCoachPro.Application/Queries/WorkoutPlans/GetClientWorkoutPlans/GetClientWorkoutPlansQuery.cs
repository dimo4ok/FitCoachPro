using FitCoachPro.Application.Common.Models.Pagination;

namespace FitCoachPro.Application.Queries.WorkoutPlans.GetClientWorkoutPlans;

public record GetClientWorkoutPlansQuery(Guid ClientId, PaginationParams PaginationParams);

