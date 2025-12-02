using FitCoachPro.Application.Common.Models.Exercise;

namespace FitCoachPro.Application.Common.Models.TemplateWorkoutItem;

public record CreateTemplateWorkoutItemModel(string Description, Guid ExerciseId);