using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public class DomainErrors
{
    public static Error Forbidden => new("Forbidden", "You do not have access to this entity.");
    public static Error NotFound(string entity) => new(entity, "The requested entity was not found.");
    public static Error AlreadyExists(string entity) => new(entity, "An entity with this name or date already exists.");
    public static Error InvalidEntityId(string entity) => new(entity, "Entity with this id does not exist.");
    public static Error UsedInActiveEntity(string entity) => new (entity, "This entity is in use and cannot be deleted.");
}
