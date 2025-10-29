using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Application.Common.Models.Users
{
    public class LoginUserModel
    {
        public string UserName { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}
