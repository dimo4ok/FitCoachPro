using FitCoachPro.Application.Common.Models.Requests;
using FitCoachPro.Domain.Entities;

namespace FitCoachPro.Application.Common.Extensions;

public static class ClientCoachRequestExtenisons
{
    public static ClientCoachRequestModel ToModel(this ClientCoachRequest request) =>
        new(
            request.Id,
            request.ClientId,
            request.CoachId,
            request.Status,
            request.Comment,
            request.CreatedAt,
            request.ReviewedAt,
            request.RowVersion);
}
