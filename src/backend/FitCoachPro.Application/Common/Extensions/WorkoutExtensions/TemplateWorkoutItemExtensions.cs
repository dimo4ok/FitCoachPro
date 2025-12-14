using FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutItem;
using FitCoachPro.Domain.Entities.Workouts.Items;

namespace FitCoachPro.Application.Common.Extensions.WorkoutExtensions;

public static class TemplateWorkoutItemExtensions 
{
    public static TemplateWorkoutItemModel ToModel(this TemplateWorkoutItem templateItem) =>
        new(
            templateItem.Id,
            templateItem.Reps,
            templateItem.Sets,
            templateItem.Description,
            templateItem.ExerciseId,
            templateItem.Exercise.ToModel());

    public static TemplateWorkoutItem ToEntity(this CreateTemplateWorkoutItemModel model) =>
        new()
        {
            Reps = model.Reps,
            Sets = model.Sets,
            Description = model.Description,
            ExerciseId = model.ExerciseId
        };

    public static TemplateWorkoutItem ToEntity(this UpdateTemplateWorkoutItemModel model) =>
        new()
        {
            Reps = model.Reps,
            Sets = model.Sets,
            Description = model.Description,
            ExerciseId = model.ExerciseId
        };
}
