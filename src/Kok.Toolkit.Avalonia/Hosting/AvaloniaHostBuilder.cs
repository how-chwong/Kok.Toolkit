using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kok.Toolkit.Avalonia.Hosting
{
    /// <summary>
    /// Avalonia宿主构建器
    /// </summary>
    public class AvaloniaHostBuilder
    {
        private readonly IHostBuilder _builder;

        /// <summary>
        /// 构造一个Avalonia宿主构建器
        /// </summary>
        /// <param name="args">启动参数</param>
        public AvaloniaHostBuilder(string[] args)
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
        public AvaloniaHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> services)
        {
            _builder.ConfigureServices(services);
            return this;
        }

        /// <summary>
        /// 注入服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public AvaloniaHostBuilder ConfigureServices(Action<IServiceCollection> services)
        {
            _builder.ConfigureServices(services);

            return this;
        }

        public AvaloniaHostBuilder ConfigureServices(Action<IServiceCollection, IConfiguration> services)
        {
            _builder.ConfigureServices((context, collection) =>
            {
                services(collection, context.Configuration);
            });

            return this;
        }

        /// <summary>
        /// 注入JSON格式的配置文件
        /// </summary>
        /// <param name="fileName">配置文件名称</param>
        /// <returns></returns>
        public AvaloniaHostBuilder AddJsonConfiguration(string fileName)
        {
            _builder.ConfigureAppConfiguration(d =>
                d.AddJsonFile(fileName, false, true));
            return this;
        }

        /// <summary>
        /// 构建宿主
        /// </summary>
        /// <returns></returns>
        public AvaloniaHost Build()
        {
            var host = _builder.Build();
            Ioc.Default.ConfigureServices(host.Services);
            return new AvaloniaHost(host);
        }
    }
}
