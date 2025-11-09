using FitCoachPro.Application.Common.Models.Exercise;
using FitCoachPro.Application.Common.Models.WorkoutItem;
using FitCoachPro.Application.Common.Models.WorkoutPlan;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Items;
using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Application.Common.Extensions;

public static class WorkoutExtensions
{
    public static WorkoutPlanModel ToModel(this WorkoutPlan workoutPlan)
    {
        return new WorkoutPlanModel(
                workoutPlan.Id,
                workoutPlan.WorkoutDate,
                workoutPlan.WorkoutItems.Select(x => x.ToModel())
                );
    }

    public static WorkoutItemModel ToModel(this WorkoutItem workoutItem)
    {
        return new WorkoutItemModel(
                workoutItem.Id,
                workoutItem.Description,
                workoutItem.ExerciseId,
                workoutItem.Exercise.ToModel()
                );
    }

    public static ExerciseModel ToModel(this Exercise exercise)
    {
        return new ExerciseModel(
                exercise.Id,
                exercise.ExerciseName,
                exercise.GifUrl
                );
    }
}
