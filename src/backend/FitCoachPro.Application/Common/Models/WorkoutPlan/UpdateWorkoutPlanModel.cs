using FitCoachPro.Application.Common.Models.WorkoutItem;

namespace FitCoachPro.Application.Common.Models.WorkoutPlan;

public record UpdateWorkoutPlanModel(DateTime WorkoutDate, IEnumerable<UpdateWorkoutItemModel> WorkoutItems);
