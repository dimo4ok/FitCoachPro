using Microsoft.EntityFrameworkCore.Storage;

namespace FitCoachPro.Application.Interfaces.Repository;

public interface IUnitOfWork
{
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}