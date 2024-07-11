using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Kok.Test.AvaloniaDemo.Views;
using Kok.Toolkit.Avalonia.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;

namespace Kok.Test.AvaloniaDemo
{
    public partial class App : Application
    {
        public override void RegisterServices()
        {
            base.RegisterServices();
            if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _host = new AvaloniaHost(desktop.Args ?? Array.Empty<string>()).ConfigureServices(AddServices);
        }

        public override void Initialize()
        {
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
                if (_host != null)
                {
                    await _host.StartAsync();
                    _host?.Run<MainWindow>(desktop);
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void AddServices(IServiceCollection services)
        {
            services.AddNavigationService();
            services.AddDialogService();
            services.AddViewModels();
            services.AddViews("Kok.Test.AvaloniaDemo.Views");
        }

        public static string GetStringArg(IConfiguration configuration, CommandArgType type)
        {
            return type switch
            {
                CommandArgType.ConfigFile => configuration["c"] ?? string.Empty,
                CommandArgType.IOConfig => configuration["io"] ?? string.Empty,
                _ => string.Empty
            };
        }

        public static bool GetBoolArg(IConfiguration configuration, CommandArgType type)
        {
            return type switch
            {
                CommandArgType.LogTelegram => "1".Equals(configuration["log"]),
                _ => false
            };
        }
    }

    public enum CommandArgType
    {
        
        ConfigFile,

        IOConfig,

        LogTelegram
    }
}
