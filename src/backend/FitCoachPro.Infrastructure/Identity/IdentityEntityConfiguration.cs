using FitCoachPro.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations
{
    public class IdentityEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserName).
                IsRequired();
            builder.HasIndex(x => x.UserName)
                .IsUnique();

            builder.Property(x => x.Email)
                .IsRequired();
            builder.HasIndex(x =>x.Email)
                .IsUnique();

            builder.Property(x => x.PhoneNumber);
            builder.HasIndex(x => x.PhoneNumber)
                .IsUnique();

            builder.Property(x => x.PasswordHash)
                .IsRequired();

            builder.HasOne(x => x.DomainUser)
                .WithOne()
                .HasForeignKey<ApplicationUser>(x => x.DomainUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
