using FitCoachPro.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Application.Common.Models.Users
{
    public class AppUserModel
    {
        public Guid Id { get; init; } 
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public UserRole Role { get; init; }

        public string Email { get; init; } = null!;
        public string UserName { get; init; } = null!;
        public string Password { get; set; } = null!;
    }
}
