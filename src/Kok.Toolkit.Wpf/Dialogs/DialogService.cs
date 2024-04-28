using CommunityToolkit.Mvvm.DependencyInjection;
using System.Collections.Concurrent;
using System.Windows;

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
            if (!temp.IsVisible)
                temp.Show();
            temp.BringIntoView();
            temp.Activate();
            return;
        }

        var win = Ioc.Default.GetService<T>();
        if (win != null)
        {
            win.Closed += (sender, e) =>
            {
                if (sender != null)
                    _windows.TryRemove(sender.GetType(), out _);
            };
            _windows.TryAdd(typeof(T), win);
            win.Show();
        }
        else
        {
            MessageBox.Show($"尝试打开{typeof(T)}窗体失败，未在容器中发现该类型!", "错误", MessageBoxButton.OK);
        }
    }

    ///<inheritdoc />
    public bool? ShowDialog<T>() where T : Window
    {
        var win = Ioc.Default.GetService<T>();
        if (win != null)
            return win.ShowDialog();

        MessageBox.Show($"尝试打开{typeof(T)}窗体失败，未在容器中发现该类型!", "错误", MessageBoxButton.OK);
        return false;
    }

    ///<inheritdoc />
    public async Task<bool?> ShowDialogAsync<T>(object? parameter) where T : Window, IWithParameterWindow
    {
        var win = Ioc.Default.GetService<T>();
        if (win != null)
        {
            await win.InitializeAsync(parameter);
            return win.ShowDialog();
        }
        MessageBox.Show($"尝试打开{typeof(T)}窗体失败，未在容器中发现该类型!", "错误", MessageBoxButton.OK);
        return false;
    }
}
