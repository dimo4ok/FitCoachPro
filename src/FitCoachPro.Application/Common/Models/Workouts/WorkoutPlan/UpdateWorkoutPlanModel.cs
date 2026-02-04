using FitCoachPro.Application.Common.Models.Workouts.WorkoutItem;

namespace FitCoachPro.Application.Common.Models.Workouts.WorkoutPlan;

public record UpdateWorkoutPlanModel(DateTime WorkoutDate, IEnumerable<UpdateWorkoutItemModel> WorkoutItems);
