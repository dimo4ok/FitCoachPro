using FitCoachPro.Domain.Entities.Users;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Domain.Entities.Workouts.Items
{
    public class TemplateWorkoutItem : BaseWorkoutItem
    {
        public Guid TemplateWorkoutPlanId { get; set; }
        public TemplateWorkoutPlan TemplateWorkoutPlan { get; set; } = null!;
    }
}
