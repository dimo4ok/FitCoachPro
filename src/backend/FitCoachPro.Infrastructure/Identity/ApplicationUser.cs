using FitCoachPro.Domain.Entities.Users;
using FitCoachPro.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid DomainUserId { get; set; }
        public User DomainUser { get; set; } = null!;
    }
}
