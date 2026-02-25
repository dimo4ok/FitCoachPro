using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Commands.Users.UpdateCoachAcceptingNewClients;

public record UpdateCoachAcceptingNewClientsCommand(ClientAcceptanceStatus AcceptanceStatus);
