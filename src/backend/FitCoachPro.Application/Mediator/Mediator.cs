using FitCoachPro.Application.Mediator.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FitCoachPro.Application.Mediator;

public class Mediator(IServiceScopeFactory scopeFactory) : IMediator
{
    public async Task ExecuteCommandAsync<T>(T command, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var commandHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<T>>();

        await commandHandler.ExecuteAsync(command, cancellationToken);
    }

    public async Task<TResult> ExecuteCommandAsync<T, TResult>(T command, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var commandHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<T, TResult>>();

        return await commandHandler.ExecuteAsync(command, cancellationToken);
    }

    public async Task ExecuteQueryAsync<T>(T query, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var queryHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<T>>();

        await queryHandler.ExecuteAsync(query, cancellationToken);
    }

    public async Task<TResult> ExecuteQueryAsync<T, TResult>(T query, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var queryHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<T, TResult>>();

        return await queryHandler.ExecuteAsync(query, cancellationToken);
    }
}
