using FitCoachPro.Application.Common.Models.Workouts.WorkoutItem;

namespace FitCoachPro.Application.Common.Models.Workouts.WorkoutPlan;

public record WorkoutPlanModel(Guid Id, DateTime WorkoutDate, IEnumerable<WorkoutItemModel> WorkoutItems);