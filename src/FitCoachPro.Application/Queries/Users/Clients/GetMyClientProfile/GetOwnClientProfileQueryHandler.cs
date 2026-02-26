using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Queries.Users.Clients.GetMyClientProfile;

public class GetOwnClientProfileQueryHandler(
    IUserRepository userRepository,
    IUserContextService userContext,
    ILogger<GetOwnClientProfileQueryHandler> logger
    ) : IQueryHandler<GetMyClientProfileQuery, Result<ClientPrivateProfileModel>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserContextService _userContext = userContext;
    private readonly ILogger<GetOwnClientProfileQueryHandler> _logger = logger;

    public async Task<Result<ClientPrivateProfileModel>> ExecuteAsync(GetMyClientProfileQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Client)
        {
            _logger.LogWarning(
                "GetMyClientProfile forbidden: User is not a Client. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result<ClientPrivateProfileModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "GetMyClientProfile attempt started. ClientId: {ClientId}",
            currentUser.UserId);

        var client = await _userRepository.GetClientByIdAsync(currentUser.UserId, cancellationToken);
        if (client is null)
        {
            _logger.LogWarning(
                "GetMyClientProfile failed: Client not found. ClientId: {ClientId}",
                currentUser.UserId);
            return Result<ClientPrivateProfileModel>.Fail(DomainErrors.NotFound(nameof(Client)), StatusCodes.Status404NotFound);
        }

        _logger.LogInformation(
            "GetMyClientProfile succeeded. ClientId: {ClientId}",
            currentUser.UserId);

        return Result<ClientPrivateProfileModel>.Success(client.ToPrivateModel());
    }
}
