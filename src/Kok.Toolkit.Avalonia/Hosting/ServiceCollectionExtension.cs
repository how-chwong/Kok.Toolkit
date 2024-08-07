﻿using Kok.Toolkit.Avalonia.Dialogs;
using Kok.Toolkit.Avalonia.Mvvm;
using Kok.Toolkit.Avalonia.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace Kok.Toolkit.Avalonia.Hosting;

/// <summary>
/// 容器扩展
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// 向IOC容器注入继承了<see cref="ViewModel"/>的视图模型
    /// </summary>
    /// <param name="services">服务集合/容器</param>
    /// <returns></returns>
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        var vms = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsAbstract && (
                type.IsSubclassOf(typeof(ViewModel)) ||
                type.IsSubclassOf(typeof(MessengerViewModel)) ||
                type.IsSubclassOf(typeof(MessengerViewModel<>))));
        foreach (var type in vms) services.AddTransient(type);
        return services;
    }

    /// <summary>
    /// 向IOC容器注入指定命名空间下的视图
    /// </summary>
    /// <param name="services">服务集合/容器</param>
    /// <param name="nameSpacePrefix">待注入对象的命名空间前缀</param>
    /// <returns></returns>
    public static IServiceCollection AddViews(this IServiceCollection services, string nameSpacePrefix)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => t.Namespace != null && t.Namespace.StartsWith(nameSpacePrefix));
        foreach (var type in types) services.AddTransient(type);

        return services;
    }

    /// <summary>
    /// 向IOC容器注入单例的对话框服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDialogService(this IServiceCollection services)
        => services.AddSingleton<IDialogService, DialogService>();

    /// <summary>
    /// 向IOC容器注入单例的导航服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddNavigationService(this IServiceCollection services)
        => services.AddSingleton<INavigationService, NavigationService>();
}
