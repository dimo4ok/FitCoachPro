using FitCoachPro.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations.UserConfigurations
{
    public class CoachEntityConfiguration : IEntityTypeConfiguration<Coach>
    {
        public void Configure(EntityTypeBuilder<Coach> builder)
        {
            builder.HasMany(x => x.Clients)
                .WithOne(x => x.Coach)
                .HasForeignKey(x => x.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.TemplateWorkoutItems)
                .WithOne(x => x.Coach)
                .HasForeignKey(x => x.CoachId)
                .IsRequired();
        }
    }
}
