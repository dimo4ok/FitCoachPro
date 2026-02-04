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

namespace FitCoachPro.Application.Queries.Users.Clients.GetMyClientProfile;

public class GetOwnClientProfileQueryHandler(
    IUserRepository userRepository,
    IUserContextService userContext
    ) : IQueryHandler<GetMyClientProfileQuery, Result<ClientPrivateProfileModel>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserContextService _userContext = userContext;

    public async Task<Result<ClientPrivateProfileModel>> ExecuteAsync(GetMyClientProfileQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Client)
            return Result<ClientPrivateProfileModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var client = await _userRepository.GetClientByIdAsync(currentUser.UserId, cancellationToken);
        if (client is null)
            return Result<ClientPrivateProfileModel>.Fail(DomainErrors.NotFound(nameof(Client)), StatusCodes.Status404NotFound);

        return Result<ClientPrivateProfileModel>.Success(client.ToPrivateModel());
    }
}
