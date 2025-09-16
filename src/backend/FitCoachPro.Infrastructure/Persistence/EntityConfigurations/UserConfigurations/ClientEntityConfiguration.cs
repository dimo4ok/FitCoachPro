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
    public class ClientEntityConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.Property(x => x.SubscriptionExpiresAt);

            builder.HasOne(x => x.Coach)
                .WithMany(x => x.Clients)
                .HasForeignKey(x => x.CoachId);

            builder.HasMany(x => x.WorkoutPlans)
                .WithOne(x => x.Client)
                .HasForeignKey(x => x.ClientId)
                .IsRequired();
        }
    }
}
