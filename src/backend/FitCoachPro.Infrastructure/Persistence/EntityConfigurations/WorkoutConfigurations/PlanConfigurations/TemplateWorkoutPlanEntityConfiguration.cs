using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations.WorkoutConfigurations.PlanConfigurations
{
    public class TemplateWorkoutPlanEntityConfiguration : IEntityTypeConfiguration<TemplateWorkoutPlan>
    {
        public void Configure(EntityTypeBuilder<TemplateWorkoutPlan> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedAt)
               .IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.HasOne(x => x.Coach)
                .WithMany(x => x.TemplateWorkoutPlans)
                .HasForeignKey(x => x.CoachId)
                .IsRequired();
        }
    }
}
