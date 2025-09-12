using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string TelephoneNumber { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "User";
        public DateTime DateOfRegistration { get; set; } = DateTime.UtcNow;
        public DateTime? DateOfExpiredSubscription { get; set; }

        public ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();
    }
}
