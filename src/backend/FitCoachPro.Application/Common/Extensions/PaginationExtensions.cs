using FitCoachPro.Application.Common.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Application.Common.Extensions;

public static class PaginationExtensions
{
    public static async Task<PaginatedModel<T>> PaginateAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedModel<T>(page, totalPages, pageSize, totalItems, items);
    }
}