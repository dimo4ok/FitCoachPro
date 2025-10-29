using FitCoachPro.Application.Common.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Application.Common.Models.Users
{
    public class UserErrors
    {
        public static Error EmailAlreadyExists => new("User.EmailAlreadyExists", "Email already exists.");
        public static Error NotFound => new("User.NotFound", "User not found.");
        public static Error WrongPassword => new("User.WrongPassword", "The password provided is incorrect.");
        public static Error EmptyDomainData => new("User.DomainDataMissing", "The user account is missing domain data.");
    }
}
