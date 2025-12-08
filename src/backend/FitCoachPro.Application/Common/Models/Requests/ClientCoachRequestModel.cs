using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using System;

namespace FitCoachPro.Application.Common.Models.Requests;

public record ClientCoachRequestModel(
    Guid Id,
    Guid ClientId,
    Guid CoachId,
    CoachRequestStatus Status,
    string? Comment,
    DateTime CreatedAt,
    DateTime? ReviewedAt,
    byte[] RowVersion
    );
