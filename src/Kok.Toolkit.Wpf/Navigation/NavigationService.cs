using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Kok.Toolkit.Core.Extension;
using System.Windows.Controls;

namespace Kok.Toolkit.Wpf.Navigation;

/// <summary>
/// 导航服务
/// </summary>
public class NavigationService : ObservableObject, INavigationService
{
    private UserControl? _currentView;

    public UserControl? CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }

    ///<inheritdoc />
    public void ToView<T>() where T : UserControl => NavigateTo(typeof(T));

    ///<inheritdoc />
    public void ToViewModel<T>() where T : ObservableObject
    {
        var vmType = typeof(T).Name;
        if (!vmType.EndsWith("ViewModel")) throw new InvalidOperationException("视图模型名称必须以ViewModel结尾");
        var viewName = vmType.Replace("ViewModel", "View");
        var viewType = viewName.ToType() ?? throw new InvalidOperationException("视图名称必须以View结尾");

        NavigateTo(viewType);
    }

    private void NavigateTo(Type viewType)
    {
        if (CurrentView?.DataContext is IConfirmNavigation old && !old.OnNavigateFrom()) return;

        var obj = Ioc.Default.GetService(viewType) ?? throw new InvalidOperationException($"导航操作失败，未发现指定的{viewType.Name}视图");
        if (obj is not UserControl { DataContext: not null } control)
            throw new InvalidOperationException($"导航操作失败，视图导航仅支持指定了{nameof(UserControl.DataContext)}属性的{nameof(UserControl)}");
        if (control.DataContext is not ObservableObject vm)
            throw new InvalidOperationException($"导航操作失败，属性{nameof(UserControl.DataContext)}的值类型必须继承自{nameof(ObservableObject)}");
        if (control.DataContext is IConfirmNavigation temp && !temp.OnNavigateTo()) return;

        CurrentView = control;
    }
}
