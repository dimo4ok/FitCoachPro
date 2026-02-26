using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Queries.Users.Coaches.GetCoachClients;

public class GetCoachClientsQueryHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    ILogger<GetCoachClientsQueryHandler> logger
    ) : IQueryHandler<GetCoachClientsQuery, Result<PaginatedModel<ClientPrivateProfileModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<GetCoachClientsQueryHandler> _logger = logger;

    public async Task<Result<PaginatedModel<ClientPrivateProfileModel>>> ExecuteAsync(GetCoachClientsQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach)
        {
            _logger.LogWarning(
                "GetCoachClients forbidden: User is not a Coach. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result<PaginatedModel<ClientPrivateProfileModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "GetCoachClients attempt started. CoachId: {CoachId}, Page: {PageNumber}, Size: {PageSize}",
            currentUser.UserId, query.PaginationParams.PageNumber, query.PaginationParams.PageSize);

        var clientsQuery = _userRepository.GetAllCoachClientsAsQuery(currentUser.UserId);
        if (!await clientsQuery.AnyAsync(cancellationToken))
        {
            _logger.LogWarning(
                "GetCoachClients failed: No clients found. CoachId: {CoachId}",
                currentUser.UserId);
            return Result<PaginatedModel<ClientPrivateProfileModel>>.Fail(DomainErrors.NotFound(nameof(Client)), StatusCodes.Status404NotFound);
        }

        var paginated = await clientsQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize,cancellationToken);

        _logger.LogInformation(
            "GetCoachClients succeeded. CoachId: {CoachId}, ReturnedCount: {Count}, Total: {Total}",
            currentUser.UserId, paginated.Items.Count, paginated.TotalItems);

        return Result<PaginatedModel<ClientPrivateProfileModel>>.Success(paginated.ToModel(x => x.ToPrivateModel()));
    }
}