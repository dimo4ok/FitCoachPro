using FitCoachPro.Application.Commands.Auth.SignUp;
using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NSubstitute;


namespace FitCoachPro.Tests.Commands.Auth;

public class SignUpCommandHandlerTests
{
    private readonly UserManager<User> _mockUserManager;
    private readonly RoleManager<IdentityRole<Guid>> _mockRoleManager;
    private readonly IUserRepository _mockRepository;
    private readonly IAuthHelper _mockAuthHelper;
    private readonly IUnitOfWork _mockUnitOfWork;
    private readonly SignUpCommandHandler _handler;

    public SignUpCommandHandlerTests()
    {
        _mockUserManager = MockFactory.GetMockUserManager<User>();
        _mockRoleManager = MockFactory.GetMockRoleManager();
        _mockRepository = Substitute.For<IUserRepository>();
        _mockUnitOfWork = Substitute.For<IUnitOfWork>();
        _mockAuthHelper = Substitute.For<IAuthHelper>();

        _handler = new SignUpCommandHandler(
            _mockUserManager,
            _mockRoleManager,
            _mockRepository,
            _mockUnitOfWork,
            _mockAuthHelper
            );
    }



    [Fact]
    public async Task ExecuteAsync_IfEmailAlreadyExists_ReturnsFailResult()
    {
        //Arrange
        var command = TestDataFactory.GetSignUpCommand();
        var existingUser = TestDataFactory.GetUser();

        _mockUserManager.FindByEmailAsync(command.Model.Email).Returns(existingUser);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.EmailAlreadyExists, result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_IfCreateUserFails_ReturnsFailResult()
    {
        //Arrange
        var command = TestDataFactory.GetSignUpCommand();

        _mockUserManager.FindByEmailAsync(command.Model.Email).Returns((User?)null);

        _mockUserManager.CreateAsync(Arg.Any<User>(), command.Model.Password)
            .Returns(IdentityResult.Failed(new IdentityError
            {
                Code = "AnyError",
                Description = "Something went wrong"
            }));

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors!);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_IfRoleNotExists_ReturnsFailResult()
    {
        //Arrange
        var command = TestDataFactory.GetSignUpCommand();

        _mockUserManager.FindByEmailAsync(command.Model.Email).Returns((User?)null);
        _mockUserManager.CreateAsync(Arg.Any<User>(), command.Model.Password).Returns(IdentityResult.Success);

        _mockRoleManager.RoleExistsAsync(command.Model.Role.ToString()).Returns(false);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.RoleNotFound, result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_IfAddRoleFail_ReturnsFailResult()
    {
        //Arrange
        var command = TestDataFactory.GetSignUpCommand();

        _mockUserManager.FindByEmailAsync(command.Model.Email).Returns((User?)null);
        _mockUserManager.CreateAsync(Arg.Any<User>(), command.Model.Password).Returns(IdentityResult.Success);
        _mockRoleManager.RoleExistsAsync(command.Model.Role.ToString()).Returns(true);

        _mockUserManager.AddToRoleAsync(Arg.Any<User>(), command.Model.Role.ToString())
            .Returns(IdentityResult.Failed(new IdentityError
            {
                Code = "AnyError",
                Description = "Something went wrong"
            }));

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Errors!);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_IfAllValid_ReturnsSuccessResult()
    {
        //Arrange
        var command = TestDataFactory.GetSignUpCommand();
        var createdDomainUserId = Guid.NewGuid();
        var authModel = TestDataFactory.GetAuthModel();

        _mockUserManager.FindByEmailAsync(command.Model.Email).Returns((User?)null);
        _mockUserManager.CreateAsync(Arg.Any<User>(), command.Model.Password).Returns(IdentityResult.Success);
        _mockRoleManager.RoleExistsAsync(command.Model.Role.ToString()).Returns(true);
        _mockUserManager.AddToRoleAsync(Arg.Any<User>(), command.Model.Role.ToString()).Returns(IdentityResult.Success);
        _mockRepository.CreateAsync(Arg.Any<CreateUserModel>(), Arg.Any<CancellationToken>()).Returns(createdDomainUserId);

        _mockAuthHelper.GenerateTokenByData(Arg.Any<JwtPayloadModel>()).Returns(authModel);

        //Act
        var result = await _handler.ExecuteAsync(command, Arg.Any<CancellationToken>());

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(authModel.Token, result.Data!.Token);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
    }
}
