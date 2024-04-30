using CommunityToolkit.Mvvm.DependencyInjection;
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

        var win = Ioc.Default.GetService<T>();
        if (win == null) throw new Exception($"尝试打开{nameof(T)}窗体失败，未在容器中发现该类型!");

        win.Closed += (sender, e) =>
        {
            if (sender != null)
                _windows.TryRemove(sender.GetType(), out _);
        };
        _windows.TryAdd(typeof(T), win);
        win.Show();
    }

    ///<inheritdoc />
    public bool? ShowDialog<T>() where T : Window
    {
        var win = Ioc.Default.GetService<T>();
        if (win == null) throw new Exception($"尝试打开{nameof(T)}窗体失败，未在容器中发现该类型!");
        return win.ShowDialog();
    }

    ///<inheritdoc />
    public async Task<bool?> ShowDialogAsync<T>(object? parameter) where T : Window, IWithParameterWindow
    {
        var win = Ioc.Default.GetService<T>();
        if (win == null) throw new Exception($"尝试打开{nameof(T)}窗体失败，未在容器中发现该类型!");

        await win.InitializeAsync(parameter);
        return await Dispatcher.CurrentDispatcher.InvokeAsync(() => win.ShowDialog());
    }

    public async Task ShowDialogAsync<TView, TViewModel>(object? parameter = null, Action<TViewModel>? callback = null)
        where TView : Window, IWithParameterWindow
        where TViewModel : class
    {
        var win = Ioc.Default.GetService<TView>();
        var vm = Ioc.Default.GetService<TViewModel>();
        if (win == null || vm == null)
            throw new Exception($"尝试打开{nameof(TView)}/{nameof(TViewModel)}窗体失败，未在容器中发现该类型!");

        win.DataContext = vm;
        await win.InitializeAsync(parameter);
        var result = await Dispatcher.CurrentDispatcher.InvokeAsync(() => win.ShowDialog());
        if (result == true) callback?.Invoke(vm);
    }
}
