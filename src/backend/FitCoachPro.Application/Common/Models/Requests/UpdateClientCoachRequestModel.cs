using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Common.Models.Requests;

public  record UpdateClientCoachRequestModel(CoachRequestStatus status, byte[] RowVersion);
