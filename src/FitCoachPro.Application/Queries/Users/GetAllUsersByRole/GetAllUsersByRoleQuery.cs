using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Queries.Users.GetAllUsersByRole;

public record GetAllUsersByRoleQuery(PaginationParams PaginationParams, UserRole Role);
