using Microsoft.Extensions.Hosting;

namespace Kok.Toolkit.Avalonia.Hosting;

public abstract class AvaloniaHostedService : IHostedService
{
    protected AvaloniaHostedService(IHostApplicationLifetime appLifetime)
    {
        appLifetime.ApplicationStarted.Register(OnStart);
        appLifetime.ApplicationStopping.Register(OnStopping);
        appLifetime.ApplicationStopped.Register(OnStopped);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected abstract void OnStart();

    protected abstract void OnStopping();

    protected abstract void OnStopped();
}
