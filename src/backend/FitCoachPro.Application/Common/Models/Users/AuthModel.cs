using FitCoachPro.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Application.Common.Models.Users
{
    public class AuthModel
    {
        public string Token { get; init; } = null!;
        public DateTime Expires { get; init; }
        public Guid UserId { get; init; }
        public string UserName { get; init; } = null!;
        public UserRole Role { get; init; }
    }
}
