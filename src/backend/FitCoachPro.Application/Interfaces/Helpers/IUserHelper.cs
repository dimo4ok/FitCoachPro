using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Domain.Entities.Identity;
using FitCoachPro.Domain.Entities.Users;

namespace FitCoachPro.Application.Interfaces.Helpers;

public interface IUserHelper
{
    Task<Result> AssignCoachToClientAsync(Guid clientId, Guid coachId, CancellationToken cancellationToken = default);
    Task<Result> DeleteAccountAsync(UserProfile user, CancellationToken cancellationToken);
    Task<Result> UnassignCoachAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task<Result> UpdateEmailAsync(User user, string newEmail);
    Task<Result> UpdatePasswordAsync(User user, UpdatePasswordModel model);
    Task<Result> UpdatePhoneNumberAsync(User user, string? newPhoneNumber);
}