using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.Users.UnassignClient;

public class UnassignClientCommandHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    ICoachAssignmentService coachAssignmentService,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<UnassignClientCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICoachAssignmentService _coachAssignmentService = coachAssignmentService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> ExecuteAsync(UnassignClientCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach)
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        if(!await _userRepository.CanCoachAccessClientAsync(currentUser.UserId, command.ClientId, cancellationToken))
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var unassignResult = await _coachAssignmentService.UnassignCoachAsync(command.ClientId, cancellationToken);
        if(!unassignResult.IsSuccess)
            return Result.Fail(unassignResult.Errors!, unassignResult.StatusCode);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
