using FitCoachPro.Domain.Entities.Users;
using FitCoachPro.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations.UserConfigurations;

public class ClientEntityConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.Property(x => x.SubscriptionExpiresAt);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne<AppUser>()
            .WithOne()
            .HasForeignKey<Client>(x => x.UserId);

        builder.HasOne(x => x.Coach)
            .WithMany(x => x.Clients)
            .HasForeignKey(x => x.CoachId);

        builder.HasMany(x => x.WorkoutPlans)
            .WithOne(x => x.Client)
            .HasForeignKey(x => x.ClientId)
            .IsRequired();
    }
}
