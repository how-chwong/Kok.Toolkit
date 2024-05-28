using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Controls;

namespace Kok.Toolkit.Wpf.Navigation;

/// <summary>
/// 导航接口
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// 当前显示的视图
    /// </summary>
    UserControl? CurrentView { get; }

    /// <summary>
    /// 导航到指定视图
    /// </summary>
    /// <typeparam name="T">视图的类型</typeparam>
    void ToView<T>() where T : UserControl;

    /// <summary>
    /// 导航到指定视图模型
    /// </summary>
    /// <typeparam name="T">视图模型的类型</typeparam>
    void ToViewModel<T>() where T : ObservableObject;
}

/// <summary>
/// 需要确认的导航接口
/// </summary>
public interface IConfirmNavigation
{
    /// <summary>
    /// 当导航到当前模型时的行为
    /// </summary>
    /// <returns>true:可以进入该视图;false:无法进入视图，导航失败</returns>
    bool OnNavigateTo();

    /// <summary>
    /// 当从当前模型导航到其他模型时的行为
    /// </summary>
    /// /// <returns>true:可以离开该视图;false:无法离开视图，导航失败</returns>
    bool OnNavigateFrom();
}
