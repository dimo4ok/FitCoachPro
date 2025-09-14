using FitCoachPro.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Domain.Entities.Users
{
    public class Admin : User
    {
        public override UserRole Role { get; protected set; } = UserRole.Admin;
    }
}
