using FitCoachPro.Application.Common.Models.Workouts.WorkoutItem;
using FitCoachPro.Domain.Entities.Workouts.Items;

namespace FitCoachPro.Application.Common.Extensions.WorkoutExtensions;

public static class WorkoutItemExtension
{
    public static WorkoutItem ToEntity(this CreateWorkoutItemModel model) =>
       new()
       {
           Reps = model.Reps,
           Sets = model.Sets,
           Description = model.Description,
           ExerciseId = model.ExerciseId
       };

    public static WorkoutItem ToEntity(this UpdateWorkoutItemModel model) =>
        new()
        {
            Reps = model.Reps,
            Sets = model.Sets,
            Description = model.Description,
            ExerciseId = model.ExerciseId,
        };

    public static WorkoutItemModel ToModel(this WorkoutItem workoutItem) =>
     new(
         workoutItem.Id,
         workoutItem.Reps,
         workoutItem.Sets,
         workoutItem.Description,
         workoutItem.ExerciseId,
         workoutItem.Exercise.ToModel());
}
