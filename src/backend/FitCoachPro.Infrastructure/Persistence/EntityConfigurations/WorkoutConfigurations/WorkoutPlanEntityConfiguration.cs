using FitCoachPro.Domain.Entities.Workouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations.WorkoutConfigurations
{
    public class WorkoutPlanEntityConfiguration : IEntityTypeConfiguration<WorkoutPlan>
    {
        public void Configure(EntityTypeBuilder<WorkoutPlan> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.DateOfDoing)
                .IsRequired();

            builder.HasOne(x => x.Client)
                .WithMany(x => x.WorkoutPlans)
                .HasForeignKey(x => x.ClientId)
                .IsRequired();

            builder.HasMany(x => x.WorkoutItems)
                .WithOne(x => x.WorkoutPlan)
                .HasForeignKey(x => x.WorkoutPlanId)
                .IsRequired();
        }
    }
}
