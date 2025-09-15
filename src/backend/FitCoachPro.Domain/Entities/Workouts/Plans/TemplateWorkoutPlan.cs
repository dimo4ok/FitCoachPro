using FitCoachPro.Domain.Entities.Users;
using FitCoachPro.Domain.Entities.Workouts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Domain.Entities.Workouts.Plans
{
    public class TemplateWorkoutPlan : BaseWorkoutPlan
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Guid CoachId { get; set; }
        public Coach Coach { get; set; } = null!;

        public ICollection<TemplateWorkoutItem> TemplateWorkoutItems { get; set; } = new List<TemplateWorkoutItem>();
    }
}
