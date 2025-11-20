using FitCoachPro.Application.Common.Models.WorkoutItem;

namespace FitCoachPro.Application.Common.Models.WorkoutPlan;

public record UpdateWorkoutPlanModel(DateTime WorkoutDate, Guid ClientId, IEnumerable<UpdateWorkoutItemModel> WorkoutItems);
