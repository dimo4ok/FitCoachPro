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

namespace FitCoachPro.Application.Queries.Users.Clients.GetClientProfileById;

public class GetClientProfileByIdQueryHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    IUsersAccessService accessService
    ) : IQueryHandler<GetClientProfileByIdQuery, Result<ClientPublicProfileModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUsersAccessService _accessService = accessService;

    public async Task<Result<ClientPublicProfileModel>> ExecuteAsync(GetClientProfileByIdQuery query, CancellationToken cancellationToken)
    {
        if (!_accessService.HasCurrentUserAccessToUsers(_userContext.Current.Role, UserRole.Client))
            return Result<ClientPublicProfileModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var client = await _userRepository.GetClientByIdAsync(query.Id, cancellationToken);
        if (client is null)
            return Result<ClientPublicProfileModel>.Fail(DomainErrors.NotFound(nameof(Client)), StatusCodes.Status404NotFound);

        return Result<ClientPublicProfileModel>.Success(client.ToPublicModel());
    }
}
