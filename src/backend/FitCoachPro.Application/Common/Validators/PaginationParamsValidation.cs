using FitCoachPro.Application.Common.Models.Pagination;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators;

public class PaginationParamsValidation : AbstractValidator<PaginationParams>
{
    public PaginationParamsValidation()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
