namespace FitCoachPro.Application.Common.Models.Pagination;

public record PaginatedModel<T>(int Page, int TotalPages, int PageSize, int TotalItems, IReadOnlyList<T> Items);
