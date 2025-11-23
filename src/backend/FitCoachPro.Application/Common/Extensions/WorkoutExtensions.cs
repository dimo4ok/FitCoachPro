using FitCoachPro.Application.Common.Models.Exercise;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.WorkoutItem;
using FitCoachPro.Application.Common.Models.WorkoutPlan;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Items;
using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Application.Common.Extensions;

public static class WorkoutExtensions
{
    public static WorkoutPlan ToEntity(this CreateWorkoutPlanModel model)
    {
        return new WorkoutPlan
        {
            WorkoutDate = model.WorkoutDate,
            ClientId = model.ClientId,
            WorkoutItems = model.WorkoutItems.Select(x => x.ToEntity()).ToList()
        };
    }

    public static WorkoutItem ToEntity(this CreateWorkoutItemModel model)
    {
        return new WorkoutItem
        {
            Description = model.Description,
            ExerciseId = model.ExerciseId
        };
    }

    public static WorkoutItem ToEntity(this UpdateWorkoutItemModel model)
    {
        return new WorkoutItem
        {
            Description = model.Description,
            ExerciseId = model.ExerciseId,
        };
    }

    public static WorkoutPlanModel ToModel(this WorkoutPlan workoutPlan)
    {
        return new WorkoutPlanModel(
                workoutPlan.Id,
                workoutPlan.WorkoutDate,
                workoutPlan.WorkoutItems.Select(x => x.ToModel()).ToList().AsReadOnly());
    }

    public static IReadOnlyList<WorkoutPlanModel> ToModel(this IReadOnlyList<WorkoutPlan> workoutPlans)
    {
        return workoutPlans.Select(x => x.ToModel()).ToList().AsReadOnly();
    }

    public static PaginatedModel<WorkoutPlanModel> ToModel(this PaginatedModel<WorkoutPlan> paginated)
    {
        return new PaginatedModel<WorkoutPlanModel>(
            paginated.Page, 
            paginated.TotalPages, 
            paginated.PageSize, 
            paginated.TotalItems, 
            paginated.Items.ToModel());
    }

    public static WorkoutItemModel ToModel(this WorkoutItem workoutItem)
    {
        return new WorkoutItemModel(
                workoutItem.Id,
                workoutItem.Description,
                workoutItem.ExerciseId,
                workoutItem.Exercise.ToModel());
    }

    public static ExerciseModel ToModel(this Exercise exercise)
    {
        return new ExerciseModel(
                exercise.Id,
                exercise.ExerciseName,
                exercise.GifUrl);
    }
}
