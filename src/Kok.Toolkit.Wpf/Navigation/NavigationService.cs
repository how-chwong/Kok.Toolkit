using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kok.Toolkit.Wpf.Navigation;

/// <summary>
/// 导航服务
/// </summary>
public class NavigationService : ObservableObject, INavigationService
{
    private ObservableObject? _currentView;

    public ObservableObject? CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }

    public void NavigateTo<T>() where T : ObservableObject
    {
        var vm = Ioc.Default.GetService<T>();
        if (vm == null) throw new InvalidOperationException($"导航操作失败，未发现指定的{nameof(T)}视图模型");

        if (CurrentView is IConfirmNavigation old && !old.OnNavigateFrom()) return;

        if (vm is IConfirmNavigation view && !view.OnNavigateTo()) return;

        CurrentView = vm;
    }

    public void NavigateTo(string view)
    {
        var type = Type.GetType(view);
        if (type == null) throw new InvalidOperationException($"导航操作失败，未能从名称{view}匹配的视图类");

        if (CurrentView is IConfirmNavigation old && !old.OnNavigateFrom()) return;

        var obj = Ioc.Default.GetService(type);
        if (obj == null) throw new InvalidOperationException($"导航操作失败，未发现指定的{nameof(type)}视图");
        if (obj is not FrameworkContentElement { DataContext: not null } control) throw new InvalidOperationException($"导航操作失败，视图{nameof(type)}未指定DataContext");
        if (control.DataContext is not ObservableObject vm)
            throw new InvalidOperationException($"导航操作失败，视图{nameof(type)}指定DataContext类型非法");
        if (control.DataContext is IConfirmNavigation temp && !temp.OnNavigateTo()) return;

        CurrentView = vm;
    }
}
