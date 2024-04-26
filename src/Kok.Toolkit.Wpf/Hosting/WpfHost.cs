using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Kok.Toolkit.Wpf.Hosting;

public class WpfHost : IDisposable
{
    private IHost? _host;
    private readonly IHostBuilder _builder;

    public WpfHost(string[] args)
    {
        _builder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder =>
            {
                builder.Sources.Clear();
                builder.AddCommandLine(args);
            });
    }

    public WpfHost ConfigureServices(Action<HostBuilderContext, IServiceCollection> services)
    {
        _builder.ConfigureServices(services);
        return this;
    }

    public WpfHost ConfigureServices(Action<IServiceCollection> services)
    {
        _builder.ConfigureServices(services);
        return this;
    }

    public WpfHost AddJsonConfiguration(string fileName)
    {
        _builder.ConfigureAppConfiguration(d =>
            d.AddJsonFile(fileName, false, true));
        return this;
    }

    public async Task StartAsync()
    {
        _host = _builder.Build();
        Ioc.Default.ConfigureServices(_host.Services);
        await _host.StartAsync();
    }

    public void Run<T>(string[] args) where T : Window
    {
        var win = Ioc.Default.GetService<T>();
        if (win != null) win.Show();
        else MessageBox.Show($"初始化失败，未发现指定启动类型：{typeof(T).Name}");
    }

    public async Task StopAsync()
    {
        if (_host != null)
        {
            await _host.StopAsync();
        }
    }

    public void Dispose() => _host?.Dispose();
}
