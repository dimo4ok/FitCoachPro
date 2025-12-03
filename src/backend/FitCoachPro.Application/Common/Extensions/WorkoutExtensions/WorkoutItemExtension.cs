using FitCoachPro.Application.Common.Models.WorkoutItem;
using FitCoachPro.Domain.Entities.Workouts.Items;

namespace FitCoachPro.Application.Common.Extensions.WorkoutExtensions;

public static class WorkoutItemExtension
{
    public static WorkoutItem ToEntity(this CreateWorkoutItemModel model) =>
       new()
       {
           Description = model.Description,
           ExerciseId = model.ExerciseId
       };

    public static WorkoutItem ToEntity(this UpdateWorkoutItemModel model) =>
        new()
        {
            Description = model.Description,
            ExerciseId = model.ExerciseId,
        };

    public static WorkoutItemModel ToModel(this WorkoutItem workoutItem) =>
     new(
         workoutItem.Id,
         workoutItem.Description,
         workoutItem.ExerciseId,
         workoutItem.Exercise.ToNestedModel());
}
