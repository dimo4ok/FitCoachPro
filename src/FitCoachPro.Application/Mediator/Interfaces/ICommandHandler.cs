namespace FitCoachPro.Application.Mediator.Interfaces;

public interface ICommandHandler<in T>
{
    Task ExecuteAsync(T command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in T, TResult>
{
    Task<TResult> ExecuteAsync(T command, CancellationToken cancellationToken);
}