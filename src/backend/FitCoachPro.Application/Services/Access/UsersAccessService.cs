using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Services.Access;

public class UsersAccessService : IUsersAccessService
{
    public bool HasCurrentUserAccessToUpdateProfile(UserRole currentUserRole) =>
       currentUserRole is UserRole.Admin or UserRole.Coach or UserRole.Client;

    public bool HasCurrentUserAccessToUsers(UserRole currentUserRole, UserRole usersRole) =>
        currentUserRole switch
        {
            UserRole.Admin => true,
            UserRole.Coach => usersRole == UserRole.Client,
            UserRole.Client => usersRole == UserRole.Coach,
            _ => false
        };
}