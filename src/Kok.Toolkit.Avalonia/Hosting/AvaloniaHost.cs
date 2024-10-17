using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.DependencyInjection;
using Kok.Toolkit.Avalonia.Dialogs;
using Microsoft.Extensions.Hosting;

namespace Kok.Toolkit.Avalonia.Hosting;

/// <summary>
/// Avalonia宿主
/// </summary>
public class AvaloniaHost
{
    private readonly IHost _host;

    /// <summary>
    /// 构造实例
    /// </summary>
    /// <param name="host"></param>
    public AvaloniaHost(IHost host)
    {
        _host = host;
    }

    /// <summary>
    /// 启动宿主
    /// </summary>
    /// <returns></returns>
    public async Task StartAsync() => await _host.StartAsync();

    /// <summary>
    /// 停止宿主
    /// </summary>
    /// <returns></returns>
    public async Task StopAsync() => await _host.StopAsync();

    /// <summary>
    /// 运行指定的窗体
    /// </summary>
    /// <typeparam name="T">窗体类型</typeparam>
    /// <param name="desktop">桌面程序</param>
    public async Task Run<T>(IClassicDesktopStyleApplicationLifetime desktop) where T : Window
    {
        var win = Ioc.Default.GetService<T>();
        if (win == null)
        {
            MessageBox.ShowAsync($"初始化失败，未发现指定启动类型：{typeof(T).Name},启动参数：{desktop.Args}");
            return;
        }
        desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
        desktop.MainWindow = win;
        desktop.MainWindow.Closing += async (_, _) =>
        {
            using (_host) await _host.StopAsync();
        };
        await StartAsync();
    }

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
