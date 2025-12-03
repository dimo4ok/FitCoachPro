using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace FitCoachPro.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) 
        => await _dbContext.Database.BeginTransactionAsync(cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) 
        => await _dbContext.SaveChangesAsync(cancellationToken);
}
