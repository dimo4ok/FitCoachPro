namespace FitCoachPro.Domain.Entities
{
    public class WorkoutPlan
    {
        public Guid Id { get; set; }
        public DateTime DateOfDoing { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<WorkoutItem> WorkoutItems { get; set; } = new List<WorkoutItem>();
    }
}