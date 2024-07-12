using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.DependencyInjection;
using Kok.Toolkit.Avalonia.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kok.Toolkit.Avalonia.Hosting;

/// <summary>
/// Avalonia宿主
/// </summary>
public class AvaloniaHost : IDisposable
{
    private IHost? _host;
    private readonly IHostBuilder _builder;

    /// <summary>
    /// 构造一个Avalonia宿主
    /// </summary>
    /// <param name="args">启动参数</param>
    public AvaloniaHost(string[] args)
    {
        _builder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder =>
            {
                builder.Sources.Clear();
                builder.AddCommandLine(args);
            });
    }

    /// <summary>
    /// 配置服务注入行为
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public AvaloniaHost ConfigureServices(Action<HostBuilderContext, IServiceCollection> services)
    {
        _builder.ConfigureServices(services);
        return this;
    }

    /// <summary>
    /// 注入服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public AvaloniaHost ConfigureServices(Action<IServiceCollection> services)
    {
        _builder.ConfigureServices(services);
        return this;
    }

    /// <summary>
    /// 注入JSON格式的配置文件
    /// </summary>
    /// <param name="fileName">配置文件名称</param>
    /// <returns></returns>
    public AvaloniaHost AddJsonConfiguration(string fileName)
    {
        _builder.ConfigureAppConfiguration(d =>
            d.AddJsonFile(fileName, false, true));
        return this;
    }

    /// <summary>
    /// 启动宿主
    /// </summary>
    /// <returns></returns>
    public async Task StartAsync()
    {
        _host = _builder.Build();
        Ioc.Default.ConfigureServices(_host.Services);
        await _host.StartAsync();
    }

    /// <summary>
    /// 运行指定的窗体
    /// </summary>
    /// <typeparam name="T">窗体类型</typeparam>
    /// <param name="desktop">桌面程序</param>
    public void Run<T>(IClassicDesktopStyleApplicationLifetime desktop) where T : Window
    {
        var win = Ioc.Default.GetService<T>();
        if (win == null)
        {
            MessageBox.ShowAsync("初始化失败，未发现指定启动类型：{typeof(T).Name},启动参数：{args}");
            return;
        }
        desktop.MainWindow = win;
        desktop.MainWindow.Closing += async (_, _) =>
        {
            if (_host == null) return;
            using (_host) await _host.StopAsync();
        };
    }

    /// <summary>
    /// 停止宿主
    /// </summary>
    /// <returns></returns>
    public async Task StopAsync()
    {
        if (_host != null)
        {
            await _host.StopAsync();
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose() => _host?.Dispose();

    /// <summary>
    /// 当前宿主的桌面应用
    /// </summary>
    public static IClassicDesktopStyleApplicationLifetime Desktop
    {
        get
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                return (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!;
            throw new InvalidOperationException("仅支持桌面样式的应用获取Desktop");
        }
    }

    /// <summary>
    /// 当前应用的激活窗体
    /// </summary>
    public static Window? CurrentWindow => Desktop.Windows.FirstOrDefault(d => d.IsActive);

    /// <summary>
    /// 当前应用的主窗体
    /// </summary>
    public static Window? MainWindow => Desktop.MainWindow;
}
