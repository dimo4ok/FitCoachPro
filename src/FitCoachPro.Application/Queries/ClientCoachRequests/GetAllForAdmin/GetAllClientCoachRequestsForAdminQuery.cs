using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Queries.ClientCoachRequests.GetAllForAdmin;

public record GetAllClientCoachRequestsForAdminQuery(Guid UserId, PaginationParams PaginationParams, CoachRequestStatus? Status = null);