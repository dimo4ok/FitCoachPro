using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Identity;

namespace FitCoachPro.Infrastructure.Identity;

public class AppUserService(UserManager<AppUser> userManager) : IAppUserService
{
    public async Task<Result<Guid>> CreateAsync(SignUpModel model)
    {
        var existingEmail = await userManager.FindByEmailAsync(model.Email);
        if (existingEmail != null)
            return Result<Guid>.Fail(UserErrors.EmailAlreadyExists, 400);

        var appUser = new AppUser
        {
            Email = model.Email,
            UserName = model.UserName
        };

        var createUserResult = await userManager.CreateAsync(appUser, model.Password);
        if (!createUserResult.Succeeded)
            return Result<Guid>.Fail(MapErrors(createUserResult.Errors), 400);

        var addRoleResult = await userManager.AddToRoleAsync(appUser, model.Role.ToString());
        if (!addRoleResult.Succeeded)
        {
            await userManager.DeleteAsync(appUser);
            return Result<Guid>.Fail(MapErrors(addRoleResult.Errors), 400);
        }

        return Result<Guid>.Success(appUser.Id, 201);
    }

    public async Task<Result<AuthUserModel>> AuthenticateAsync(SignInModel model)
    {
        var user = await userManager.FindByNameAsync(model.UserName);
        if (user == null)
            return Result<AuthUserModel>.Fail(UserErrors.NotFound);

        var validPassword = await userManager.CheckPasswordAsync(user, model.Password);
        if (!validPassword)
            return Result<AuthUserModel>.Fail(UserErrors.WrongPassword);

        var roleString = (await userManager.GetRolesAsync(user)).FirstOrDefault();
        if (roleString == null)
            return Result<AuthUserModel>.Fail(UserErrors.NotFound);

        var authUserModel = new AuthUserModel
        {
            Id = user.Id,
            Email = user.Email!,
            UserName = user.UserName!,
            Role = Enum.Parse<UserRole>(roleString, true)
        };

        return Result<AuthUserModel>.Success(authUserModel);
    }

    private List<Error> MapErrors(IEnumerable<IdentityError> identityErrors)
    {
        return identityErrors
            .Select(x => new Error(x.Code, x.Description))
            .ToList();
    }
}
