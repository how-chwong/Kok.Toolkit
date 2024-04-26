using Kok.Test.WpfDemo.services;
using Kok.Test.WpfDemo.Views;
using Kok.Toolkit.Wpf.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Windows;

namespace Kok.Test.WpfDemo;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private WpfHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _host = new WpfHost(e.Args).ConfigureServices(AddServices);
        await _host.StartAsync();
        _host.Run<MainWindow>(e.Args);
    }

    private void AddServices(IServiceCollection services)
    {
        services.AddViewModels();
        services.AddViews("Kok.Test.WpfDemo.Views");
        services.AddSingleton<IMyService, MyService>();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        if (_host == null) return;
        using (_host) await _host.StopAsync();
    }
}
