using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Domain.Entities.Users
{
    public class Client : User
    {
        public override UserRole Role { get; protected set; } = UserRole.Client;
        public DateTime? SubscriptionExpiresAt { get; set; }

        public Guid? CoachId { get; set; }
        public Coach? Coach { get; set; }

        public ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();
    }
}
