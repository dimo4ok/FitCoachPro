using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Interfaces.Services;

public interface IUserService
{
    Task<Result> AssignCoachToClientAsync(Guid clientId, Guid coachId, CancellationToken cancellationToken = default);
}