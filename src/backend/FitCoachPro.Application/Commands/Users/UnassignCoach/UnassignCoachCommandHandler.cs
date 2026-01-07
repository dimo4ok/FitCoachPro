using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.Users.UnassignCoach;

public class UnassignCoachCommandHandler(
    IUserContextService userContext,
    IUserHelper userHelper,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<UnassignCoachCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserHelper _userHelper = userHelper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> ExecuteAsync(UnassignCoachCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Client)
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var unassignResult = await _userHelper.UnassignCoachAsync(currentUser.UserId, cancellationToken);
        if(!unassignResult.IsSuccess)
            return Result.Fail(unassignResult.Errors!, unassignResult.StatusCode);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
