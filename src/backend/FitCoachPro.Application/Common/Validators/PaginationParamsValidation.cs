using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Pagination;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators;

public class PaginationParamsValidation : AbstractValidator<PaginationParams>
{
    public PaginationParamsValidation()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage(PaginationErrors.InvalidPageNumber.Message);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).LessThanOrEqualTo(100).WithMessage(PaginationErrors.InvalidPageSize.Message);
    }
}
