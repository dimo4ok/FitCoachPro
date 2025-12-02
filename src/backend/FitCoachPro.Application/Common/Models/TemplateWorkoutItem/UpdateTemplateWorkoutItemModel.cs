using FitCoachPro.Application.Common.Models.Exercise;

namespace FitCoachPro.Application.Common.Models.TemplateWorkoutItem;

public record UpdateTemplateWorkoutItemModel(Guid? Id, string Description, Guid ExerciseId);