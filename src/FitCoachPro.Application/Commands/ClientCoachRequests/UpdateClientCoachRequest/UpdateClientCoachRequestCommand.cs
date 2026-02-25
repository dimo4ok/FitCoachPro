using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Commands.ClientCoachRequests.UpdateClientCoachRequest;

public record UpdateClientCoachRequestCommand(Guid RequstId, CoachRequestStatus Status);

