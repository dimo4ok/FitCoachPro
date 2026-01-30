using FitCoachPro.Application.Commands.WorkoutPlans.CreateWorkoutPlan;
using FitCoachPro.Application.Commands.WorkoutPlans.DeleteWorkoutPlan;
using FitCoachPro.Application.Commands.WorkoutPlans.UpdateWorkoutPlan;
using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Workouts.WorkoutItem;
using FitCoachPro.Application.Common.Models.Workouts.WorkoutPlan;
using FitCoachPro.Application.Queries.WorkoutPlans.GetClientWorkoutPlans;
using FitCoachPro.Application.Queries.WorkoutPlans.GetMyWorkoutPlans;
using FitCoachPro.Application.Queries.WorkoutPlans.GetWorkoutPlanById;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts.Plans;


namespace FitCoachPro.Tests.TestDataFactories;

public static class WorkoutPlanTestDataFactory
{
    public static UserContext GetCurrentUser(
      Guid? id = null,
      UserRole role = UserRole.Admin) =>
      new(
          id ?? Guid.NewGuid(),
          role
          );

    public static List<WorkoutPlan> GetEmptyWorkoutPlans()
    {
        var plans = new List<WorkoutPlan> { new() };

        return plans;
    }

    public static GetClientWorkoutPlansQuery GetClientWorkoutPlansQuery(
        Guid? clientId = null,
        int pageNumber = 1,
        int pageSize = 10) =>
        new(
            clientId ?? Guid.NewGuid(),
            new PaginationParams(pageNumber, pageSize)
            );

    public static GetMyWorkoutPlansQuery GetMyWorkoutPlansQuery(
       int pageNumber = 1,
       int pageSize = 10) =>
       new(
           new PaginationParams(pageNumber, pageSize)
           );

    public static GetWorkoutPlanByIdQuery GetWorkoutPlanByIdQuery() =>
       new(Guid.NewGuid());

    public static CreateWorkoutPlanCommand GetCreateWorkoutPlanCommand(
        Guid? clientId = null,
        DateTime? workoutDate = null,
        IEnumerable<CreateWorkoutItemModel>? items = null) =>
        new(
            new(
                workoutDate ?? DateTime.UtcNow,
                clientId ?? Guid.NewGuid(),
                items ?? new List<CreateWorkoutItemModel>()
                )
            );

    public static DeleteWorkoutPlanCommand GetDeleteWorkoutPlanCommand(Guid? id = null) =>
        new(id ?? Guid.NewGuid());

    public static UpdateWorkoutPlanCommand GetUpdateWorkoutPlanCommand(Guid? workoutPlanId = null, DateTime? dateTime = null) =>
        new(
            workoutPlanId ?? Guid.NewGuid(),
            new UpdateWorkoutPlanModel(
                dateTime ?? DateTime.UtcNow,
                new List<UpdateWorkoutItemModel>()
                )
            );
}
