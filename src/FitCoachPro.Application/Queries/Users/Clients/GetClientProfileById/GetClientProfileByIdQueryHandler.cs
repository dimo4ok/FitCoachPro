using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Queries.Users.Clients.GetClientProfileById;

public class GetClientProfileByIdQueryHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    IUsersAccessService accessService,
    ILogger<GetClientProfileByIdQueryHandler> logger
    ) : IQueryHandler<GetClientProfileByIdQuery, Result<ClientPublicProfileModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUsersAccessService _accessService = accessService;
    private readonly ILogger<GetClientProfileByIdQueryHandler> _logger = logger;

    public async Task<Result<ClientPublicProfileModel>> ExecuteAsync(GetClientProfileByIdQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "GetClientProfileById attempt started. ClientId: {ClientId}, CurrentUserId: {UserId}, CurrentUserRole: {Role}",
            query.Id, currentUser.UserId, currentUser.Role);

        if (!_accessService.HasCurrentUserAccessToUsers(currentUser.Role, UserRole.Client))
        {
            _logger.LogWarning(
                "GetClientProfileById forbidden. ClientId: {ClientId}, CurrentUserId: {UserId}, CurrentUserRole: {Role}",
                query.Id, currentUser.UserId, currentUser.Role);
            return Result<ClientPublicProfileModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        var client = await _userRepository.GetClientByIdAsync(query.Id, cancellationToken);
        if (client is null)
        {
            _logger.LogWarning(
                "GetClientProfileById failed: Client not found. ClientId: {ClientId}",
                query.Id);
            return Result<ClientPublicProfileModel>.Fail(DomainErrors.NotFound(nameof(Client)), StatusCodes.Status404NotFound);
        }

        _logger.LogInformation(
            "GetClientProfileById succeeded. ClientId: {ClientId}",
            query.Id);

        return Result<ClientPublicProfileModel>.Success(client.ToPublicModel());
    }
}
