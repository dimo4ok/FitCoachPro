using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Queries.WorkoutPlans.GetClientWorkoutPlans;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using FitCoachPro.Tests.TestDataFactories;
using Microsoft.AspNetCore.Http;
using MockQueryable;
using NSubstitute;

namespace FitCoachPro.Tests.Queries.WorkoutPlans;

public class GetClientWorkoutPlansQueryHandlerTests
{
    private readonly IUserContextService _mockUserContext;
    private readonly IWorkoutPlanRepository _mockRepository;
    private readonly IWorkoutPlanAccessService _mockAccessService;
    private readonly GetClientWorkoutPlansQueryHandler _handler;

    public GetClientWorkoutPlansQueryHandlerTests()
    {
        _mockUserContext = Substitute.For<IUserContextService>();
        _mockRepository = Substitute.For<IWorkoutPlanRepository>();
        _mockAccessService = Substitute.For<IWorkoutPlanAccessService>();

        _handler = new GetClientWorkoutPlansQueryHandler(
            _mockUserContext,
            _mockRepository,
            _mockAccessService
            );
    }

    [Theory]
    [InlineData(UserRole.Coach)]
    [InlineData(UserRole.Client)]
    public async Task ExecuteAsync_IfUserWithoutAccess_ReturnsFailResult(UserRole userRole)
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: userRole);
        var query = WorkoutPlanTestDataFactory.GetClientWorkoutPlansQuery();

        _mockUserContext.Current.Returns(currentUser);

        //if (userRole == UserRole.Coach)
            _mockAccessService.HasCoachAccessToWorkoutPlan(currentUser, query.ClientId, Arg.Any<CancellationToken>()).Returns(false);

        //Act
        var result = await _handler.ExecuteAsync(query, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Forbidden, result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);

        //if (userRole == UserRole.Coach)
            await _mockAccessService.Received(1).HasCoachAccessToWorkoutPlan(currentUser, query.ClientId, Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Coach)]
    public async Task ExecuteAsync_IfWorkoutPlansNotFound_ReturnsFailResult(UserRole userRole)
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: userRole);
        var query = WorkoutPlanTestDataFactory.GetClientWorkoutPlansQuery();
        var emptyPlansQuery = new List<WorkoutPlan>().BuildMock();

        _mockUserContext.Current.Returns(currentUser);

        //without if condition because ci/cd error
        //if(userRole == UserRole.Coach)
        _mockAccessService.HasCoachAccessToWorkoutPlan(currentUser, query.ClientId, Arg.Any<CancellationToken>()).Returns(true);

        _mockRepository.GetAllByUserIdAsQuery(query.ClientId).Returns(emptyPlansQuery);

        //Act
        var result = await _handler.ExecuteAsync(query, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.NotFound(nameof(WorkoutPlan)), result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);

        if (userRole == UserRole.Coach)
            await _mockAccessService.Received(1).HasCoachAccessToWorkoutPlan(currentUser, query.ClientId, Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Coach)]
    public async Task ExecuteAsync_IfAllValid_ReturnsSuccessResult(UserRole userRole)
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: userRole);
        var query = WorkoutPlanTestDataFactory.GetClientWorkoutPlansQuery();
        var plansQuery = new List<WorkoutPlan> { new(), new() }.BuildMock();

        _mockUserContext.Current.Returns(currentUser);

        //if(userRole == UserRole.Coach)
            _mockAccessService.HasCoachAccessToWorkoutPlan(currentUser, query.ClientId, Arg.Any<CancellationToken>()).Returns(true);

        _mockRepository.GetAllByUserIdAsQuery(query.ClientId).Returns(plansQuery);

        //Act
        var result = await _handler.ExecuteAsync(query, default);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Items.Count);

        if (userRole == UserRole.Coach)
            await _mockAccessService.Received(1).HasCoachAccessToWorkoutPlan(currentUser, query.ClientId, Arg.Any<CancellationToken>());
    }
}
