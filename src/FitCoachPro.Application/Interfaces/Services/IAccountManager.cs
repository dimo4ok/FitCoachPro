using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Domain.Entities.Identity;
using FitCoachPro.Domain.Entities.Users;

namespace FitCoachPro.Application.Interfaces.Services;

public interface IAccountManager
{
    Task<Result> UpdateEmailAsync(User user, string newEmail);
    Task<Result> UpdatePhoneNumberAsync(User user, string? newPhoneNumber);
    Task<Result> UpdatePasswordAsync(User user, UpdatePasswordModel model);
    Task<Result> DeleteAccountAsync(UserProfile userProfile, CancellationToken cancellationToken);
}