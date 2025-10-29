using FitCoachPro.Application.Common.Models.Response;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Interfaces.Repository;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace FitCoachPro.Infrastructure.Identity
{
    public class AppUserService(UserManager<AppUser> userManager, IUserRepository userRepository) : IAppUserService
    {
        public async Task<Result<bool>> CreateUserAsync(User domainUser, CreateUserModel model)
        {
            var existingEmail = await userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null)
                return Result<bool>.Fail(ErrorsList(UserErrors.EmailAlreadyExists), HttpType.POST, 400);

            var appUser = new AppUser
            {
                Email = model.Email,
                UserName = model.UserName,
                DomainUserId = domainUser.Id,
                DomainUser = domainUser
            };

            var identityResult = await userManager.CreateAsync(appUser, model.Password);
            if (!identityResult.Succeeded)
                return Result<bool>.Fail(MapErrors(identityResult.Errors), HttpType.POST, 400);

            var roleResult = await userManager.AddToRoleAsync(appUser, model.Role.ToString());
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(appUser);
                return Result<bool>.Fail(MapErrors(roleResult.Errors), HttpType.POST, 400);
            }

            return Result<bool>.Success(true, HttpType.POST, 201);
        }

        public async Task<Result<AppUserModel>> AuthenticateUserAsync(LoginUserModel model, CancellationToken cancellationToken = default)
        {
            var appUser = await userManager.FindByNameAsync(model.UserName);
            if (appUser == null)
                return Result<AppUserModel>
                    .Fail(ErrorsList(UserErrors.NotFound), HttpType.POST);

            var validPassword = await userManager.CheckPasswordAsync(appUser, model.Password);
            if (!validPassword)
                return Result<AppUserModel>
                    .Fail(ErrorsList(UserErrors.WrongPassword), HttpType.POST, 401);

            appUser.DomainUser = await userRepository.GetUserById(appUser.DomainUserId, cancellationToken);
            if (appUser.DomainUser == null)
                return Result<AppUserModel>
                   .Fail(ErrorsList(UserErrors.EmptyDomainData), HttpType.POST); //mb throw exception error

            var appUserModel = new AppUserModel
            {
                Id = appUser.DomainUser.Id,
                FirstName = appUser.DomainUser.FirstName,
                LastName = appUser.DomainUser.LastName,
                Role = appUser.DomainUser.Role,

                UserName = appUser.UserName,
                Email = appUser.Email
            };

            return Result<AppUserModel>.Success(appUserModel, HttpType.POST);
        }

        private List<Error> MapErrors(IEnumerable<IdentityError> identityErrors)
        {
            return identityErrors
                .Select(x => new Error(x.Code, x.Description))
                .ToList();
        }

        private List<Error> ErrorsList(Error error) => new() { error };
    }
}
