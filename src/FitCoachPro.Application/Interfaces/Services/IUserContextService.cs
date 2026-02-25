using FitCoachPro.Application.Common.Models;

namespace FitCoachPro.Application.Interfaces.Services;

public interface IUserContextService
{
    UserContext Current { get; }
}
