using Kok.Toolkit.Avalonia.Hosting;
using Microsoft.Extensions.Hosting;

namespace Kok.Test.AvaloniaDemo.Services;

public interface ITestService
{
    bool IsRunning { get; }

    void Start();

    void Stop();
}

public class TestService : ITestService
{
    public bool IsRunning { get; private set; }

    public void Start()
    {
        IsRunning = true;
    }

    public void Stop()
    {
        IsRunning = false;
    }
}

public class TestHostedService : AvaloniaHostedService
{
    private readonly ITestService _service;

    public TestHostedService(IHostApplicationLifetime appLifetime, ITestService service) : base(appLifetime)
    {
        _service = service;
    }

    protected override void OnStart()
    {
        _service.Start();
    }

    protected override void OnStopping()
    {
        _service.Stop();
    }

    protected override void OnStopped()
    {
    }
}
