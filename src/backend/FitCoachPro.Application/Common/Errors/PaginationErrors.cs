using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public static class PaginationErrors
{
    public static Error InvalidPageNumber => new("Pagination.InvalidPageNumber", "Page number must be at least 1.");
    public static Error InvalidPageSize => new("Pagination.InvalidPageSize", "Page size must be between 1 and 100.");
}
