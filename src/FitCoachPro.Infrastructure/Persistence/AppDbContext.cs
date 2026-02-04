using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Identity;
using FitCoachPro.Domain.Entities.Users;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Items;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Coach> Coaches { get; set; }
    public DbSet<Client> Clients { get; set; }

    public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
    public DbSet<WorkoutItem> WorkoutItems { get; set; }

    public DbSet<TemplateWorkoutPlan> TemplateWorkoutPlans { get; set; }
    public DbSet<TemplateWorkoutItem> TemplateWorkoutItems { get; set; }

    public DbSet<Exercise> Exercises { get; set; }

    public DbSet<ClientCoachRequest> ClientCoachRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
