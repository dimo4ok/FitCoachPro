using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Pagination;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators;

public class PaginationParamsValidation : AbstractValidator<PaginationParams>
{
    public PaginationParamsValidation()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
                .WithErrorCode(PaginationErrors.InvalidPageNumber.Code)
                .WithMessage(PaginationErrors.InvalidPageNumber.Message);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
                .WithErrorCode(PaginationErrors.InvalidPageSize.Code)
                .WithMessage(PaginationErrors.InvalidPageSize.Message);
    }
}
