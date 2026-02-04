using FitCoachPro.Application.Common.Models;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Interfaces.Services.Access;

public interface IUsersAccessService
{
    bool HasCurrentUserAccessToUpdateProfile(UserRole currentUserRole);
    bool HasCurrentUserAccessToUsers(UserRole currentUserRole, UserRole usersRole);
}