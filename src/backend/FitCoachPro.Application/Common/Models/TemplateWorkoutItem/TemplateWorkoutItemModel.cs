using FitCoachPro.Application.Common.Models.Exercise;

namespace FitCoachPro.Application.Common.Models.TemplateWorkoutItem;

public record class TemplateWorkoutItemModel(Guid Id, string Description, Guid ExercsieId, ExerciseNestedModel Exercise);
