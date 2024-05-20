using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Kok.Toolkit.Core.Extension;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

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

    ///<inheritdoc />
    public void NavigateTo<T>() where T : ObservableObject
    {
        var vm = Ioc.Default.GetService<T>() ?? throw new InvalidOperationException($"导航操作失败，未发现指定的{typeof(T).Name}视图模型");
        if (CurrentView is IConfirmNavigation old && !old.OnNavigateFrom()) return;

        if (vm is IConfirmNavigation view && !view.OnNavigateTo()) return;

        CurrentView = vm;
    }

    ///<inheritdoc />
    [Obsolete("该方法有缺陷，会导致视图构造两次")]
    public void NavigateTo(Type viewType)
    {
        if (CurrentView is IConfirmNavigation old && !old.OnNavigateFrom()) return;

        var obj = Ioc.Default.GetService(viewType) ?? throw new InvalidOperationException($"导航操作失败，未发现指定的{viewType.Name}视图");
        if (obj is not UserControl { DataContext: not null } control)
            throw new InvalidOperationException($"导航操作失败，视图{viewType.Name}未指定DataContext");
        if (control.DataContext is not ObservableObject vm)
            throw new InvalidOperationException($"导航操作失败，视图{viewType.Name}指定DataContext类型非法");
        if (control.DataContext is IConfirmNavigation temp && !temp.OnNavigateTo()) return;

        if (!viewType.IsSubclassOf(typeof(UserControl))) throw new InvalidOperationException($"当使用视图类型导航时，仅支持UserControl,请修改{viewType.Name}视图类型");

        CurrentView = vm;
    }

    public void NavigateTo(string viewName)
    {
        var type = viewName.ToType() ?? throw new InvalidOperationException($"无法识别名称为{viewName}的视图");
        NavigateTo(type!);
    }
}
