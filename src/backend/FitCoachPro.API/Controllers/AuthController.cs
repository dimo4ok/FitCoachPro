using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FitCoachPro.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]CreateUserModel model, CancellationToken cancellationToken = default)
        {
            var response = await authService.RegisterUserAsync(model, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody]LoginUserModel model, CancellationToken cancellationToken = default)
        {
            var response = await authService.LoginUserAsync(model, cancellationToken);
            return Ok(response);
        }
    }
}
