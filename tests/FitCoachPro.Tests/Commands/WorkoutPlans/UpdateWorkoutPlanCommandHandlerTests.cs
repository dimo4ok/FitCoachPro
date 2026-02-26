using FitCoachPro.Application.Commands.WorkoutPlans.UpdateWorkoutPlan;
using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Items;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using FitCoachPro.Tests.TestDataFactories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using MockQueryable;
using NSubstitute;

namespace FitCoachPro.Tests.Commands.WorkoutPlans;

public class UpdateWorkoutPlanCommandHandlerTests
{
    private readonly IUserContextService _mockUserContext;
    private readonly IWorkoutPlanRepository _mockWorkoutPlanRepository;
    private readonly IExerciseRepository _mockExerciseRepository;
    private readonly IUnitOfWork _mockUnitOfWork;
    private readonly IWorkoutPlanHelper _mockHelper;
    private readonly IWorkoutPlanAccessService _mockAccessService;
    private readonly UpdateWorkoutPlanCommandHandler _handler;

    public UpdateWorkoutPlanCommandHandlerTests()
    {
        _mockUserContext = Substitute.For<IUserContextService>();
        _mockWorkoutPlanRepository = Substitute.For<IWorkoutPlanRepository>();
        _mockExerciseRepository = Substitute.For<IExerciseRepository>();
        _mockUnitOfWork = Substitute.For<IUnitOfWork>();
        _mockHelper = Substitute.For<IWorkoutPlanHelper>();
        _mockAccessService = Substitute.For<IWorkoutPlanAccessService>();

        _handler = new UpdateWorkoutPlanCommandHandler(
            _mockUserContext,
            _mockWorkoutPlanRepository,
            _mockExerciseRepository,
            _mockUnitOfWork,
            _mockHelper,
            _mockAccessService,
            NullLogger<UpdateWorkoutPlanCommandHandler>.Instance
            );
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Coach)]
    [InlineData(UserRole.Client)]
    public async Task ExecuteAsync_IfWorkoutPlanNotFound_ReturnsFailResult(UserRole userRole)
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: userRole);
        var command = WorkoutPlanTestDataFactory.GetUpdateWorkoutPlanCommand();

        _mockUserContext.Current.Returns(currentUser);
        _mockWorkoutPlanRepository.GetByIdAsync(Arg.Is(command.WorkoutPlanId), Arg.Any<CancellationToken>(), Arg.Is(true)).Returns((WorkoutPlan?)null);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.NotFound(nameof(WorkoutPlan)), result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);

        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Coach)]
    [InlineData(UserRole.Client)]
    public async Task ExecuteAsync_IfUserWithoutAccess_ReturnsFailResult(UserRole userRole)
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: userRole);
        var command = WorkoutPlanTestDataFactory.GetUpdateWorkoutPlanCommand();
        var workoutPlan = new WorkoutPlan { Id = command.WorkoutPlanId, ClientId = Guid.NewGuid() };

        _mockWorkoutPlanRepository.GetByIdAsync(Arg.Is(command.WorkoutPlanId), Arg.Any<CancellationToken>(), Arg.Is(true)).Returns(workoutPlan);

        _mockUserContext.Current.Returns(currentUser);
        _mockAccessService.HasCoachAccessToWorkoutPlan(Arg.Is(currentUser), Arg.Is(workoutPlan.ClientId), Arg.Any<CancellationToken>()).Returns(false);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Forbidden, result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);

        await _mockAccessService.Received(1).HasCoachAccessToWorkoutPlan(Arg.Is(currentUser), Arg.Is(workoutPlan.ClientId), Arg.Any<CancellationToken>());
        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_IfWorkoutPlanAlreadyExists_ReturnsFailResult()
    {
        //Arrange
        var date = new DateTime(2026, 1, 1);
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: UserRole.Coach);
        var command = WorkoutPlanTestDataFactory.GetUpdateWorkoutPlanCommand(dateTime: date);
        var workoutPlanWithDIfferWorkoutDate = new WorkoutPlan { Id = command.WorkoutPlanId, ClientId = Guid.NewGuid(), WorkoutDate = date.AddDays(1) };

        _mockWorkoutPlanRepository.GetByIdAsync(Arg.Is(command.WorkoutPlanId), Arg.Any<CancellationToken>(), Arg.Is(true)).Returns(workoutPlanWithDIfferWorkoutDate);
        _mockUserContext.Current.Returns(currentUser);
        _mockAccessService.HasCoachAccessToWorkoutPlan(Arg.Is(currentUser), Arg.Is(workoutPlanWithDIfferWorkoutDate.ClientId), Arg.Any<CancellationToken>()).Returns(true);

        _mockWorkoutPlanRepository.ExistsByClientAndDateAsync(workoutPlanWithDIfferWorkoutDate.ClientId, command.Model.WorkoutDate, Arg.Any<CancellationToken>())
            .Returns(true);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.AlreadyExists(nameof(WorkoutPlan)), result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);

        await _mockAccessService.Received(1).HasCoachAccessToWorkoutPlan(Arg.Is(currentUser), Arg.Is(workoutPlanWithDIfferWorkoutDate.ClientId), Arg.Any<CancellationToken>());
        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_IfWorkoutPlanExerciseNotExists_ReturnsFailResult()
    {
        //Arrange
        var date = new DateTime(2026, 1, 1);
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: UserRole.Coach);
        var command = WorkoutPlanTestDataFactory.GetUpdateWorkoutPlanCommand(dateTime: date);
        var workoutPlan = new WorkoutPlan { Id = command.WorkoutPlanId, ClientId = Guid.NewGuid(), WorkoutDate = date};
        var emptyExercisesQuery = new List<Exercise>().BuildMock();

        _mockWorkoutPlanRepository.GetByIdAsync(Arg.Is(command.WorkoutPlanId), Arg.Any<CancellationToken>(), Arg.Is(true)).Returns(workoutPlan);
        _mockUserContext.Current.Returns(currentUser);
        _mockAccessService.HasCoachAccessToWorkoutPlan(Arg.Is(currentUser), Arg.Is(workoutPlan.ClientId), Arg.Any<CancellationToken>()).Returns(true);
        _mockWorkoutPlanRepository.ExistsByClientAndDateAsync(Arg.Is(workoutPlan.ClientId), Arg.Is(command.Model.WorkoutDate), Arg.Any<CancellationToken>()).Returns(true);

        _mockExerciseRepository.GetAllAsQuery().Returns(emptyExercisesQuery);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.NotFound(nameof(Exercise)), result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);

        await _mockAccessService.Received(1).HasCoachAccessToWorkoutPlan(Arg.Is(currentUser), Arg.Is(workoutPlan.ClientId), Arg.Any<CancellationToken>());
        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_IfUpdateIWorkoutItemsNotValid_ReturnsFailResult()
    {
        //Arrange
        var date = new DateTime(2026, 1, 1);
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: UserRole.Coach);
        var command = WorkoutPlanTestDataFactory.GetUpdateWorkoutPlanCommand(dateTime: date);
        var workoutPlan = new WorkoutPlan { Id = command.WorkoutPlanId, ClientId = Guid.NewGuid(), WorkoutDate = date };
        var exercisesQuery = new List<Exercise> { new() }.BuildMock();
        var exerciseIdSet = exercisesQuery.Select(x => x.Id).ToHashSet();

        _mockWorkoutPlanRepository.GetByIdAsync(Arg.Is(command.WorkoutPlanId), Arg.Any<CancellationToken>(), Arg.Is(true)).Returns(workoutPlan);
        _mockUserContext.Current.Returns(currentUser);
        _mockAccessService.HasCoachAccessToWorkoutPlan(Arg.Is(currentUser), Arg.Is(workoutPlan.ClientId), Arg.Any<CancellationToken>()).Returns(true);
        _mockWorkoutPlanRepository.ExistsByClientAndDateAsync(Arg.Is(workoutPlan.ClientId), Arg.Is(command.Model.WorkoutDate), Arg.Any<CancellationToken>()).Returns(true);
        _mockExerciseRepository.GetAllAsQuery().Returns(exercisesQuery);

        _mockHelper.ValidateUpdateItems(Arg.Any<ICollection<WorkoutItem>>(), Arg.Is(command.Model.WorkoutItems), Arg.Any<HashSet<Guid>>())
            .Returns((false, new Error("SomeErrorCode", "SomeErrorMessage")));

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Errors);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);

        await _mockAccessService.Received(1).HasCoachAccessToWorkoutPlan(Arg.Is(currentUser), Arg.Is(workoutPlan.ClientId), Arg.Any<CancellationToken>());
        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_IfAllValid_ReturnsSuccessResult()
    {
        //Arrange
        var date = new DateTime(2026, 1, 1);
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: UserRole.Coach);
        var command = WorkoutPlanTestDataFactory.GetUpdateWorkoutPlanCommand(dateTime: date);
        var workoutPlan = new WorkoutPlan { Id = command.WorkoutPlanId, ClientId = Guid.NewGuid(), WorkoutDate = date };
        var exercisesQuery = new List<Exercise> { new() }.BuildMock();
        var exerciseIdSet = exercisesQuery.Select(x => x.Id).ToHashSet();

        _mockWorkoutPlanRepository.GetByIdAsync(Arg.Is(command.WorkoutPlanId), Arg.Any<CancellationToken>(), Arg.Is(true)).Returns(workoutPlan);
        _mockUserContext.Current.Returns(currentUser);
        _mockAccessService.HasCoachAccessToWorkoutPlan(Arg.Is(currentUser), Arg.Is(workoutPlan.ClientId), Arg.Any<CancellationToken>()).Returns(true);
        _mockWorkoutPlanRepository.ExistsByClientAndDateAsync(Arg.Is(workoutPlan.ClientId), Arg.Is(command.Model.WorkoutDate), Arg.Any<CancellationToken>()).Returns(true);
        _mockExerciseRepository.GetAllAsQuery().Returns(exercisesQuery);

        _mockHelper.ValidateUpdateItems(Arg.Any<ICollection<WorkoutItem>>(), Arg.Is(command.Model.WorkoutItems), Arg.Any<HashSet<Guid>>()).Returns((true, null));

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        await _mockAccessService.Received(1).HasCoachAccessToWorkoutPlan(Arg.Is(currentUser), Arg.Is(workoutPlan.ClientId), Arg.Any<CancellationToken>());
        _mockHelper.Received(1).SyncItems(Arg.Any<ICollection<WorkoutItem>>(), Arg.Is(command.Model.WorkoutItems));
        await _mockUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

}
