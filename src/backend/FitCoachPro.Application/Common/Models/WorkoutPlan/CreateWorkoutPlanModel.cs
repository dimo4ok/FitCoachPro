using FitCoachPro.Application.Common.Models.WorkoutItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Application.Common.Models.WorkoutPlan;

public record CreateWorkoutPlanModel(DateTime WorkoutDate, Guid ClientId, IEnumerable<CreateWorkoutItemModel> WorkoutItems);
