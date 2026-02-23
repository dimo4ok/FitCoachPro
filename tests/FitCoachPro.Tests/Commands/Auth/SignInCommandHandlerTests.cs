using FitCoachPro.Application.Commands.Auth.SignIn;
using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Identity;
using FitCoachPro.Tests.TestDataFactories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace FitCoachPro.Tests.Commands.Auth;

public class SignInCommandHandlerTests
{
    private readonly UserManager<User> _mockUserManager;
    private readonly IUserRepository _mockRepository;
    private readonly IAuthHelper _mockAuthHelper;
    private readonly SignInCommandHandler _handler;

    public SignInCommandHandlerTests()
    {
        TestCleaner.Clean();

        _mockUserManager = MockFactory.GetMockUserManager<User>();
        _mockRepository = Substitute.For<IUserRepository>();
        _mockAuthHelper = Substitute.For<IAuthHelper>();

        _handler = new SignInCommandHandler(
            _mockUserManager,
            _mockRepository,
            _mockAuthHelper
            );
    }

    [Fact]
    public async Task ExecuteAsync_IfUserDoesNotExist_ReturnsFailResult()
    {
        //Arrange
        var command = AuthTestDataFactory.GetSignInCommand("unknow-user", "any");

        _mockUserManager.FindByNameAsync(command.Model.UserName).Returns((User?)null);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.NotFound, result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_IfPasswordIsIncorrect_ReturnsFailResult()
    {
        //Arrange
        var command = AuthTestDataFactory.GetSignInCommand(password: "wrong-password");
        var user = AuthTestDataFactory.GetUser();

        _mockUserManager.FindByNameAsync(command.Model.UserName).Returns(user);
        _mockUserManager.CheckPasswordAsync(user, command.Model.Password).Returns(false);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.InvalidCredentials, result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_IfUserHasNoRole_ReturnsFailResult()
    {
        //Arrange
        var command = AuthTestDataFactory.GetSignInCommand();
        var user = AuthTestDataFactory.GetUser();

        _mockUserManager.FindByNameAsync(command.Model.UserName).Returns(user);
        _mockUserManager.CheckPasswordAsync(user, command.Model.Password).Returns(true);
        _mockUserManager.GetRolesAsync(user).Returns(new List<string>());

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.RoleNotFound, result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_IfRoleIsInvalid_ReturnsFailResult()
    {
        //Arrange
        var command = AuthTestDataFactory.GetSignInCommand();
        var user = AuthTestDataFactory.GetUser();

        _mockUserManager.FindByNameAsync(command.Model.UserName).Returns(user);
        _mockUserManager.CheckPasswordAsync(user, command.Model.Password).Returns(true);
        _mockUserManager.GetRolesAsync(user).Returns(new List<string>() { "InvalidEnum" });

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.InvalidRole, result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_IfDomainUserNotFound_ReturnsFailResult()
    {
        //Arrange
        var command = AuthTestDataFactory.GetSignInCommand();
        var user = AuthTestDataFactory.GetUser();

        _mockUserManager.FindByNameAsync(command.Model.UserName).Returns(user);
        _mockUserManager.CheckPasswordAsync(user, command.Model.Password).Returns(true);
        _mockUserManager.GetRolesAsync(user).Returns(new List<string>() { $"{UserRole.Admin}" });
        _mockRepository.GetIdByAppUserIdAndRoleAsync(user.Id, Arg.Any<UserRole>(), Arg.Any<CancellationToken>())
            .Returns((Guid?)null);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.NotFound, result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_IfAllValid_ReturnsSuccessResult()
    {
        //Arrange
        var command = AuthTestDataFactory.GetSignInCommand();
        var user = AuthTestDataFactory.GetUser();
        var authModel = AuthTestDataFactory.GetAuthModel();

        _mockUserManager.FindByNameAsync(command.Model.UserName).Returns(user);
        _mockUserManager.CheckPasswordAsync(user, command.Model.Password).Returns(true);
        _mockUserManager.GetRolesAsync(user).Returns(new List<string>() { $"{UserRole.Admin}" });
        _mockRepository.GetIdByAppUserIdAndRoleAsync(user.Id, Arg.Any<UserRole>(), Arg.Any<CancellationToken>())
            .Returns(new Guid());
        
        _mockAuthHelper.GenerateTokenByData(Arg.Any<JwtPayloadModel>()).Returns(authModel);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(authModel, result.Data);
    }
}
