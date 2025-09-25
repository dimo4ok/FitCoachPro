using FitCoachPro.Application.Common.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace FitCoachPro.Application.Interfaces.Services
{
    public interface IJwtService
    {
        public string GenerateJWT(JwtUserModel jwtUserModel);
    }
}
