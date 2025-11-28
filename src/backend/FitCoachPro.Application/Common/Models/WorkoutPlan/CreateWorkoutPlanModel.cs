using FitCoachPro.Application.Common.Models.WorkoutItem;

namespace FitCoachPro.Application.Common.Models.WorkoutPlan;

public record CreateWorkoutPlanModel(DateTime WorkoutDate, Guid ClientId, IEnumerable<CreateWorkoutItemModel> WorkoutItems);
