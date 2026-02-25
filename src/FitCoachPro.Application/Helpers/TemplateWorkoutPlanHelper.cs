using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutItem;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Domain.Entities.Workouts.Items;

namespace FitCoachPro.Application.Helpers;

public class TemplateWorkoutPlanHelper : ITemplateWorkoutPlanHelper
{
    public (bool, Error?) ExercisesExist(IEnumerable<CreateTemplateWorkoutItemModel> newItems, HashSet<Guid> exercisIdsSet)
    {
        foreach (var ni in newItems)
        {
            if (!exercisIdsSet.Contains(ni.ExerciseId))
                return (false, DomainErrors.InvalidEntityId(nameof(ni.ExerciseId)));
        }

        return (true, null);
    }

    public (bool, Error?) ValidateUpdateItems(ICollection<TemplateWorkoutItem> currentItems, IEnumerable<UpdateTemplateWorkoutItemModel> newItems, HashSet<Guid> exercisIdsSet)
    {
        foreach (var ni in newItems)
        {
            if (!exercisIdsSet.Contains(ni.ExerciseId))
                return (false, DomainErrors.InvalidEntityId(nameof(ni.ExerciseId)));

            if (ni.Id.HasValue && currentItems.All(ci => ci.Id != ni.Id))
                return (false, DomainErrors.InvalidEntityId(nameof(TemplateWorkoutItem)));
        }

        return (true, null);
    }

    public void SyncItems(ICollection<TemplateWorkoutItem> currentItems, IEnumerable<UpdateTemplateWorkoutItemModel> newItems)
    {
        var itemsToRemove = currentItems.Where(ci => newItems.All(ni => ni.Id != ci.Id)).ToList();

        foreach (var i in itemsToRemove)
            currentItems.Remove(i);

        foreach (var ni in newItems)
        {
            if (!ni.Id.HasValue)
            {
                currentItems.Add(ni.ToEntity());
                continue;
            }

            var currentItemForUpdate = currentItems.FirstOrDefault(ci => ci.Id == ni.Id);
            if (currentItemForUpdate != null)
            {
                currentItemForUpdate.Description = ni.Description;
                currentItemForUpdate.ExerciseId = ni.ExerciseId;
                continue;
            }
        }
    }
}
