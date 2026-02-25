using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public static class ValidationErrors
{
    public static Error DateCannotBeInPast => new("ValidationError.DateCannotBeInPast", $"Workout date cannot be in the past.");
    public static Error DuplicateId(string entity) => new($"ValidationError.DuplicateId.{entity}", $"The {entity} collection contains duplicate identifiers.");
    public static Error CollectionSizeInvalid(string entity) => 
        new($"ValidationError.CollectionSizeInvalid.{entity}", $"The {entity} collection must contain at least one and at most ten items.");
}
