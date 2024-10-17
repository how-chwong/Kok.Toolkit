using Kok.Toolkit.Avalonia.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace Kok.Test.AvaloniaDemo.Services;

public interface ITestService
{
    bool IsRunning { get; }

    void Start();

    void Stop();

    Action<bool> OnStatusChanged { get; set; }
}

public class TestService : ITestService
{
    public bool IsRunning { get; private set; }

    public void Start()
    {
        IsRunning = true;
        OnStatusChanged?.Invoke(IsRunning);
    }

    public void Stop()
    {
        IsRunning = false;
        OnStatusChanged?.Invoke(IsRunning);
    }

    public Action<bool> OnStatusChanged { get; set; }
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
