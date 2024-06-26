﻿using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Kok.Toolkit.Wpf.Hosting;

/// <summary>
/// WPF宿主
/// </summary>
public class WpfHost : IDisposable
{
    private IHost? _host;
    private readonly IHostBuilder _builder;

    /// <summary>
    /// 构造一个WPF宿主
    /// </summary>
    /// <param name="args">启动参数</param>
    public WpfHost(string[] args)
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
    public WpfHost ConfigureServices(Action<HostBuilderContext, IServiceCollection> services)
    {
        _builder.ConfigureServices(services);
        return this;
    }

    /// <summary>
    /// 注入服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public WpfHost ConfigureServices(Action<IServiceCollection> services)
    {
        _builder.ConfigureServices(services);
        return this;
    }

    /// <summary>
    /// 注入JSON格式的配置文件
    /// </summary>
    /// <param name="fileName">配置文件名称</param>
    /// <returns></returns>
    public WpfHost AddJsonConfiguration(string fileName)
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
    /// <param name="args">启动参数</param>
    public void Run<T>(string[] args) where T : Window
    {
        var win = Ioc.Default.GetService<T>();
        if (win != null) win.Show();
        else MessageBox.Show($"初始化失败，未发现指定启动类型：{typeof(T).Name},启动参数：{args}");
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
}
