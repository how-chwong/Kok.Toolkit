﻿using CommunityToolkit.Mvvm.DependencyInjection;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Threading;

namespace Kok.Toolkit.Wpf.Dialogs;

/// <summary>
/// 对话框服务
/// </summary>
public sealed class DialogService : IDialogService
{
    private readonly ConcurrentDictionary<Type, Window> _windows = new();

    ///<inheritdoc />
    public void Show<T>() where T : Window
    {
        if (_windows.TryGetValue(typeof(T), out var temp))
        {
            if (temp.WindowState == WindowState.Minimized)
                temp.WindowState = WindowState.Normal;
            if (!temp.IsVisible) temp.Show();
            temp.BringIntoView();
            temp.Activate();
            return;
        }

        var win = Ioc.Default.GetService<T>() ?? throw new Exception($"尝试打开{typeof(T).Name}窗体失败，未在容器中发现该类型!");
        win.Owner = Application.Current.MainWindow;
        win.Closed += (sender, e) =>
        {
            if (sender != null)
                _windows.TryRemove(sender.GetType(), out _);
        };
        _windows.TryAdd(typeof(T), win);
        win.Show();
    }

    ///<inheritdoc />
    public async Task<bool?> ShowDialogAsync<T>() where T : Window
        => await ShowDialogInternalAsync<T>();

    ///<inheritdoc />
    public async Task ShowDialogAsync<T>(Action<object>? callback) where T : Window
        => await ShowDialogInternalAsync<T>(null, callback);

    public async Task ShowDialogAsync<T>(object? parameter, Action<object>? callback) where T : Window, IWithParameterWindow
        => await ShowDialogInternalAsync<T>(parameter, callback);

    ///<inheritdoc />
    public async Task<bool?> ShowDialogAsync<T>(object? parameter) where T : Window, IWithParameterWindow
        => await ShowDialogInternalAsync<T>(parameter);

    /// <summary>
    /// 带参模态弹窗
    /// </summary>
    /// <typeparam name="TView">窗口类型</typeparam>
    /// <param name="parameter">参数</param>
    /// <param name="callback">弹窗确认后的处理函数</param>
    /// <returns></returns>
    private async Task<bool?> ShowDialogInternalAsync<TView>(object? parameter = null, Action<object>? callback = null)
        where TView : Window
    {
        var win = Ioc.Default.GetService<TView>() ?? throw new InvalidOperationException($"尝试打开{typeof(TView).Name}窗体失败，未在容器中发现该类型!"); ;
        win.Owner = Application.Current.MainWindow;
        if (win is IWithParameterWindow temp)
            await temp.InitializeAsync(parameter);
        var result = await Dispatcher.CurrentDispatcher.InvokeAsync(() => win.ShowDialog());
        if (result == true) callback?.Invoke(win.DataContext);
        return result;
    }
}
