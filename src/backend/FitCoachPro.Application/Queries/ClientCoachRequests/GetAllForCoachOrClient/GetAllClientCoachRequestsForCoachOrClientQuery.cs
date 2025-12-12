using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Queries.ClientCoachRequests.GetAllClientCoachRequestsForCoachOrClient;

public record GetAllClientCoachRequestsForCoachOrClientQuery(PaginationParams PaginationParams, CoachRequestStatus? Status = null);
