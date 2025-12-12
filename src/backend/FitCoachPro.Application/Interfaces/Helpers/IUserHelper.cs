using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Interfaces.Helpers;

public interface IUserHelper
{
    Task<Result> AssignCoachToClientAsync(Guid clientId, Guid coachId, CancellationToken cancellationToken = default);
}