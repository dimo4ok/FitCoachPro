using FitCoachPro.Application.Common.Models.Workouts.WorkoutItem;

namespace FitCoachPro.Application.Common.Models.Workouts.WorkoutPlan;

public record CreateWorkoutPlanModel(DateTime WorkoutDate, Guid ClientId, IEnumerable<CreateWorkoutItemModel> WorkoutItems);
