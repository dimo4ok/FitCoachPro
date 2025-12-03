using FitCoachPro.Application.Common.Models.TemplateWorkoutItem;
using FitCoachPro.Domain.Entities.Workouts.Items;

namespace FitCoachPro.Application.Common.Extensions.WorkoutExtensions;

public static class TemplateWorkoutItemExtensions 
{
    public static TemplateWorkoutItemModel ToModel(this TemplateWorkoutItem templateItem) =>
        new(
            templateItem.Id,
            templateItem.Description,
            templateItem.ExerciseId,
            templateItem.Exercise.ToNestedModel());

    public static TemplateWorkoutItem ToEntity(this CreateTemplateWorkoutItemModel model) =>
        new()
        {
            Description = model.Description,
            ExerciseId = model.ExerciseId
        };

    public static TemplateWorkoutItem ToEntity(this UpdateTemplateWorkoutItemModel model) =>
        new()
        {
            Description = model.Description,
            ExerciseId = model.ExerciseId
        };
}
