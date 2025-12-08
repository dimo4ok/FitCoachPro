using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Requests;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Application.Services;

public class ClientCoachRequestService(
    IUserContextService userContext,
    IUnitOfWork unitOfWork,
    IClientCoachRequestRepository requestRepository,
    IUserService userService
        ) : IClientCoachRequestService
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;
    private readonly IUserService _userService = userService;

    public async Task<Result<ClientCoachRequestModel>> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        var request = await _requestRepository.GetByIdAsync(requestId, cancellationToken);
        if (request == null)
            return Result<ClientCoachRequestModel>.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));

        var currentUser = _userContext.Current;
        if (!HasAccesToRequest(request, currentUser))
            return Result<ClientCoachRequestModel>.Fail(DomainErrors.Forbidden, 403);

        return Result<ClientCoachRequestModel>.Success(request.ToModel());
    }

    public async Task<Result<PaginatedModel<ClientCoachRequestModel>>> GetAllForCoachOrClientAsync(PaginationParams paginationParams, CoachRequestStatus? status = null, CancellationToken cancellationToken = default)
    {
        var currentUser = _userContext.Current;

        if (currentUser.Role != UserRole.Coach && currentUser.Role != UserRole.Client)
            return Result<PaginatedModel<ClientCoachRequestModel>>.Fail(DomainErrors.Forbidden, 403);

        var query = _requestRepository.GetAllByUserIdAndUserRoleAsQuery(currentUser.UserId, currentUser.Role);
        if (status != null)
            query = query.Where(x => x.Status == status);

        if (!await query.AnyAsync(cancellationToken))
            return Result<PaginatedModel<ClientCoachRequestModel>>.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));

        var paginated = await query.PaginateAsync(paginationParams.PageNumber, paginationParams.PageSize, cancellationToken);

        return Result<PaginatedModel<ClientCoachRequestModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }

    public async Task<Result<PaginatedModel<ClientCoachRequestModel>>> GetAllForAdminAsync(Guid userId, PaginationParams paginationParams, CoachRequestStatus? status = null, CancellationToken cancellationToken = default)
    {
        if (_userContext.Current.Role != UserRole.Admin)
            return Result<PaginatedModel<ClientCoachRequestModel>>.Fail(DomainErrors.Forbidden, 403);

        var query = _requestRepository.GetAllByUserIdAsQuery(userId);
        if (status != null)
            query = query.Where(x => x.Status == status);

        if (!await query.AnyAsync(cancellationToken))
            return Result<PaginatedModel<ClientCoachRequestModel>>.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));

        var paginated = await query.PaginateAsync(paginationParams.PageNumber, paginationParams.PageSize, cancellationToken);

        return Result<PaginatedModel<ClientCoachRequestModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }

    public async Task<Result> CreateAsync(Guid coachId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _userContext.Current.UserId;

        if (_userContext.Current.Role != UserRole.Client)
            return Result.Fail(DomainErrors.Forbidden, 403);

        if (!await _requestRepository.IsCoachAcceptingNewClientsAsync(coachId, cancellationToken))
            return Result.Fail(ClientCoachRequestErrors.CoachNotAcceptingNewClients, 409);

        if (!await _requestRepository.IsClientAvailableForNewCoachAsync(currentUserId, cancellationToken))
            return Result.Fail(ClientCoachRequestErrors.ClientAlreadyHasCoach, 409);

        if(await _requestRepository.IsDuplicateRequestAsync(currentUserId, coachId, cancellationToken))
            return Result.Fail(ClientCoachRequestErrors.PendingRequestAlreadyExists, 409);

        var request = new ClientCoachRequest
        {
            ClientId = currentUserId,
            CoachId = coachId,
            Status = CoachRequestStatus.Pending
        };

        await _requestRepository.CreateAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(201);
    }

    public async Task<Result> UpdateAsync(Guid requstId, UpdateClientCoachRequestModel model, CancellationToken cancellationToken = default)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach)
            return Result.Fail(DomainErrors.Forbidden, 403);

        var request = await _requestRepository.GetByIdAsync(requstId, cancellationToken, track: true);
        if (request == null)
            return Result.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));

        if (!HasAccesToRequest(request, currentUser))
            return Result.Fail(DomainErrors.Forbidden, 403);

        if (request.Status != CoachRequestStatus.Pending)
            return Result.Fail(ClientCoachRequestErrors.CannotUpdateFinalizedRequest, 409);

        if(!request.RowVersion.SequenceEqual(model.RowVersion))
            return Result.Fail(SystemErrors.ConcurrencyConflict, 409);

        if (model.status != CoachRequestStatus.Accepted)
        {
            request.Status = model.status;
            request.ReviewedAt = DateTime.UtcNow;
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        await using var transaction =  await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            request.Status = model.status;
            request.ReviewedAt = DateTime.UtcNow;

            var clientUpdateResult = await _userService.AssignCoachToClientAsync(request.ClientId, request.CoachId, cancellationToken);
            if(!clientUpdateResult.IsSuccess)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Fail(clientUpdateResult.Errors!, clientUpdateResult.StatusCode);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Result> CancelRequestAsync(Guid requestId, DeleteClientCoachRequestModel model, CancellationToken cancellationToken = default)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Client)
            return Result.Fail(DomainErrors.Forbidden, 403);

        var request = await _requestRepository.GetByIdAsync(requestId, cancellationToken, track: true);
        if (request == null)
            return Result.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));

        if (!HasAccesToRequest(request, currentUser))
            return Result.Fail(DomainErrors.Forbidden, 403);

        if(request.Status != CoachRequestStatus.Pending)
            return Result.Fail(ClientCoachRequestErrors.CannotUpdateFinalizedRequest, 409);

        if(!request.RowVersion.SequenceEqual(model.RowVersion))
            return Result.Fail(SystemErrors.ConcurrencyConflict, 409);

        _requestRepository.Delete(request);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(204);
    }

    public async Task<Result> DeleteOwnRequestsAsync(CancellationToken cancellationToken = default)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach && currentUser.Role != UserRole.Client)
            return Result.Fail(DomainErrors.Forbidden, 403);

        var requests = await _requestRepository
            .GetAllByUserIdAndUserRoleAsQuery(currentUser.UserId, currentUser.Role, track: true)
            .ToListAsync(cancellationToken);
        if (requests.Count == 0)
            return Result.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));

        _requestRepository.DeleteRequests(requests);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(203);
    }

    private bool HasAccesToRequest(ClientCoachRequest request, UserContext userContext) =>
        userContext.Role switch
        {
            UserRole.Admin => true,
            UserRole.Coach => request.CoachId == userContext.UserId,
            UserRole.Client => request.ClientId == userContext.UserId,
            _ => false
        };
}
