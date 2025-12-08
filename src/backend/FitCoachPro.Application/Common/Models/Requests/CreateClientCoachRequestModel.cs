using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Common.Models.Requests;

public record CreateClientCoachRequestModel(
    Guid ClientId,
    Guid CoachId
    );
