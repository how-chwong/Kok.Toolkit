using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Kok.Test.AvaloniaDemo.Views;
using Kok.Toolkit.Avalonia.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;

namespace Kok.Test.AvaloniaDemo
{
    public partial class App : Application
    {
        public override async void Initialize()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                _host = new AvaloniaHost(desktop.Args ?? Array.Empty<string>()).ConfigureServices(AddServices);
                await _host.StartAsync();
            }
            AvaloniaXamlLoader.Load(this);
        }

        private AvaloniaHost? _host;

        public override async void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);

                desktop.MainWindow = Ioc.Default.GetService<MainWindow>();
                desktop.MainWindow!.Closing += MainWindow_Closed;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private async void MainWindow_Closed(object? sender, System.EventArgs e)
        {
            if (_host == null) return;
            using (_host) await _host.StopAsync();
        }

        private void AddServices(IServiceCollection services)
        {
            services.AddNavigationService();
            services.AddDialogService();
            services.AddViewModels();
            services.AddViews("Kok.Test.AvaloniaDemo.Views");
        }
    }
}
