using FitCoachPro.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Domain.Entities.Workouts
{
    public class TemplateWorkoutItem : BaseWorkoutItem
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Guid CoachId { get; set; }
        public Coach Coach { get; set; } = null!;
    }
}
