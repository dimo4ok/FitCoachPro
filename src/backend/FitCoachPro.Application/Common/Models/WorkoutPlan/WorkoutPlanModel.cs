using FitCoachPro.Application.Common.Models.WorkoutItem;

namespace FitCoachPro.Application.Common.Models.WorkoutPlan;

public record WorkoutPlanModel(Guid Id, DateTime WorkoutDate, IEnumerable<WorkoutItemModel> WorkoutItems);