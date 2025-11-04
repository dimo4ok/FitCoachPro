using FitCoachPro.Domain.Entities.Workouts.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations.WorkoutConfigurations.ItemConfigurations;

public class TemplateWorkoutItemEntityConfiguration : IEntityTypeConfiguration<TemplateWorkoutItem>
{
    public void Configure(EntityTypeBuilder<TemplateWorkoutItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Description)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(x => x.TemplateWorkoutPlan)
               .WithMany(c => c.TemplateWorkoutItems)
               .HasForeignKey(x => x.TemplateWorkoutPlanId)
               .IsRequired();

        builder.HasOne(x => x.Exercise)
               .WithMany()
               .HasForeignKey(x => x.ExerciseId)
               .IsRequired();
    }
}
