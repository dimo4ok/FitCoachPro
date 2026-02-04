using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Interfaces.Services;

public interface ICoachAssignmentService
{
    Task<Result> AssignCoachToClientAsync(Guid clientId, Guid coachId, CancellationToken cancellationToken = default);
    Task<Result> UnassignCoachAsync(Guid clientId, CancellationToken cancellationToken = default);
}