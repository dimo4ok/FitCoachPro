using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public static class ClientCoachRequestErrors
{
    public static Error CoachNotAcceptingNewClients =>new("ClientCoachRequestError.CoachNotAcceptingNewClients", "This coach is currently not accepting new clients.");
    public static Error ClientAlreadyHasCoach => new("ClientCoachRequestErrors.ClientAlreadyHasCoach", "The client already has an assigned coach.");
    public static Error CannotUpdateFinalizedRequest =>
        new("ClientCoachRequestErrors.CannotUpdateFinalizedRequest", "Only requests with a 'Pending' status can be modified.");
    public static Error PendingRequestAlreadyExists =>
        new("ClientCoachRequestErrors.PendingRequestAlreadyExists", "A pending request to this coach already exists.");
}
