using FitCoachPro.Application.Common.Models.Workouts.WorkoutPlan;
using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Application.Common.Extensions.WorkoutExtensions;

public static class WorkoutPlanExtensions
{
    public static WorkoutPlan ToEntity(this CreateWorkoutPlanModel model) =>
      new()
      {
          WorkoutDate = model.WorkoutDate,
          ClientId = model.ClientId,
          WorkoutItems = model.WorkoutItems
              .Select(x => x.ToEntity())
              .ToList()
      };

    public static WorkoutPlanModel ToModel(this WorkoutPlan workoutPlan) =>
        new(
            workoutPlan.Id,
            workoutPlan.WorkoutDate,
            workoutPlan.WorkoutItems
                .Select(x => x.ToModel())
                .ToList()
                .AsReadOnly());

    public static IReadOnlyList<WorkoutPlanModel> ToModel(this IReadOnlyList<WorkoutPlan> workoutPlans) =>
        workoutPlans
            .Select(x => x.ToModel())
            .ToList()
            .AsReadOnly();
}
