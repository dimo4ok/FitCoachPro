using FitCoachPro.Domain.Entities.Identity;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations.IdentityConfigurations;

public class IdentityEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName).
            IsRequired();
        builder.HasIndex(x => x.UserName)
            .IsUnique();

        builder.Property(x => x.Email)
            .IsRequired();
        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.PhoneNumber);
        builder.HasIndex(x => x.PhoneNumber)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.HasOne<Coach>()
           .WithOne(x => x.User)
           .HasForeignKey<Coach>(x => x.UserId)
           .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Client>()
            .WithOne(x => x.User)
            .HasForeignKey<Client>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Admin>()
            .WithOne(x => x.User)
            .HasForeignKey<Admin>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
