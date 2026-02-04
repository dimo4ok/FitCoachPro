using FitCoachPro.Application.Commands.WorkoutPlans.CreateWorkoutPlan;
using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Workouts.WorkoutItem;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using FitCoachPro.Tests.TestDataFactories;
using Microsoft.AspNetCore.Http;
using MockQueryable;
using NSubstitute;

namespace FitCoachPro.Tests.Commands.WorkoutPlans;

public class CreateWorkoutPlanCommandHandlerTests
{
    private readonly IUserContextService _mockUserContext;
    private readonly IWorkoutPlanRepository _mockWorkoutPlanRepository;
    private readonly IExerciseRepository _mockExerciseRepository;
    private readonly IUnitOfWork _mockUnitOfWork;
    private readonly IWorkoutPlanHelper _mockHelper;
    private readonly IWorkoutPlanAccessService _mockAccessService;
    private readonly CreateWorkoutPlanCommandHandler _handler;

    public CreateWorkoutPlanCommandHandlerTests()
    {
        _mockUserContext = Substitute.For<IUserContextService>();
        _mockWorkoutPlanRepository = Substitute.For<IWorkoutPlanRepository>();
        _mockExerciseRepository = Substitute.For<IExerciseRepository>();
        _mockUnitOfWork = Substitute.For<IUnitOfWork>();
        _mockHelper = Substitute.For<IWorkoutPlanHelper>();
        _mockAccessService = Substitute.For<IWorkoutPlanAccessService>();

        _handler = new CreateWorkoutPlanCommandHandler(
            _mockUserContext,
            _mockWorkoutPlanRepository,
            _mockExerciseRepository,
            _mockUnitOfWork,
            _mockHelper,
            _mockAccessService
            );
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Coach)]
    [InlineData(UserRole.Client)]
    public async Task ExecuteAsync_IfUserWithoutAccess_ReturnsFailResult(UserRole userRole)
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: userRole);
        var command = WorkoutPlanTestDataFactory.GetCreateWorkoutPlanCommand();

        _mockUserContext.Current.Returns(currentUser);

        _mockAccessService.HasCoachAccessToWorkoutPlan(currentUser, command.Model.ClientId, Arg.Any<CancellationToken>()).Returns(false);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Forbidden, result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);

        await _mockAccessService.Received(1).HasCoachAccessToWorkoutPlan(currentUser, command.Model.ClientId, Arg.Any<CancellationToken>());
        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_IfWorkoutPlanAlreadyExists_ReturnsFailResult()
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: UserRole.Coach);
        var command = WorkoutPlanTestDataFactory.GetCreateWorkoutPlanCommand();

        _mockUserContext.Current.Returns(currentUser);
        _mockAccessService.HasCoachAccessToWorkoutPlan(currentUser, command.Model.ClientId, Arg.Any<CancellationToken>()).Returns(true);

        _mockWorkoutPlanRepository.ExistsByClientAndDateAsync(command.Model.ClientId, command.Model.WorkoutDate, Arg.Any<CancellationToken>()).Returns(true);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.AlreadyExists(nameof(WorkoutPlan)), result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);

        await _mockAccessService.Received(1).HasCoachAccessToWorkoutPlan(currentUser, command.Model.ClientId, Arg.Any<CancellationToken>());
        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_IfWorkoutPlanExerciseNotExists_ReturnsFailResult()
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: UserRole.Coach);
        var command = WorkoutPlanTestDataFactory.GetCreateWorkoutPlanCommand();
        var emptyExercisesQuery = new List<Exercise>().BuildMock();

        _mockUserContext.Current.Returns(currentUser);
        _mockAccessService.HasCoachAccessToWorkoutPlan(currentUser, command.Model.ClientId, Arg.Any<CancellationToken>()).Returns(true);
        _mockWorkoutPlanRepository.ExistsByClientAndDateAsync(command.Model.ClientId, command.Model.WorkoutDate, Arg.Any<CancellationToken>()).Returns(false);

        _mockExerciseRepository.GetAllAsQuery().Returns(emptyExercisesQuery);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.NotFound(nameof(Exercise)), result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);

        await _mockAccessService.Received(1).HasCoachAccessToWorkoutPlan(currentUser, command.Model.ClientId, Arg.Any<CancellationToken>());
        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_IfExercsiesNotValid_ReturnsFailResult()
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: UserRole.Coach);
        var command = WorkoutPlanTestDataFactory.GetCreateWorkoutPlanCommand();
        var exercisesQuery = new List<Exercise> { new() }.BuildMock();
        var exerciseIdSet = exercisesQuery.Select(x => x.Id).ToHashSet();

        _mockUserContext.Current.Returns(currentUser);
        _mockAccessService.HasCoachAccessToWorkoutPlan(currentUser, command.Model.ClientId, Arg.Any<CancellationToken>()).Returns(true);
        _mockWorkoutPlanRepository.ExistsByClientAndDateAsync(command.Model.ClientId, command.Model.WorkoutDate, Arg.Any<CancellationToken>()).Returns(false);
        _mockExerciseRepository.GetAllAsQuery().Returns(exercisesQuery);

        _mockHelper.ExercisesExist(Arg.Any<IEnumerable<CreateWorkoutItemModel>>(), Arg.Any<HashSet<Guid>>())
            .Returns((false, new Error("SomeErrorCode", "SomeErrorMessage")));

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);

        await _mockAccessService.Received(1).HasCoachAccessToWorkoutPlan(currentUser, command.Model.ClientId, Arg.Any<CancellationToken>());
        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_IfAllValid_ReturnsFailResult()
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: UserRole.Coach);
        var command = WorkoutPlanTestDataFactory.GetCreateWorkoutPlanCommand();
        var exercisesQuery = new List<Exercise> { new() }.BuildMock();
        var exerciseIdSet = exercisesQuery.Select(x => x.Id).ToHashSet();

        _mockUserContext.Current.Returns(currentUser);
        _mockAccessService.HasCoachAccessToWorkoutPlan(currentUser, command.Model.ClientId, Arg.Any<CancellationToken>()).Returns(true);
        _mockWorkoutPlanRepository.ExistsByClientAndDateAsync(command.Model.ClientId, command.Model.WorkoutDate, Arg.Any<CancellationToken>()).Returns(false);
        _mockExerciseRepository.GetAllAsQuery().Returns(exercisesQuery);
        _mockHelper.ExercisesExist(Arg.Any<IEnumerable<CreateWorkoutItemModel>>(), Arg.Any<HashSet<Guid>>()).Returns((true, null));

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);

        await _mockAccessService.Received(1).HasCoachAccessToWorkoutPlan(currentUser, command.Model.ClientId, Arg.Any<CancellationToken>());
        await _mockWorkoutPlanRepository.Received(1).CreateAsync(Arg.Any<WorkoutPlan>(), Arg.Any<CancellationToken>());
        await _mockUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
