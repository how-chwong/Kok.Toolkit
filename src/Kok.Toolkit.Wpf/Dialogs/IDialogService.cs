using System.Windows;

namespace Kok.Toolkit.Wpf.Dialogs;

/// <summary>
/// 对话框服务接口
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// 非模态弹窗
    /// </summary>
    /// <typeparam name="T"></typeparam>
    void Show<T>() where T : Window;

    /// <summary>
    /// 模态弹窗
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    bool? ShowDialog<T>() where T : Window;

    /// <summary>
    /// 带参模态弹窗
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameter"></param>
    /// <returns></returns>
    Task<bool?> ShowDialogAsync<T>(object? parameter)
        where T : Window, IWithParameterWindow;

    /// <summary>
    /// 带参模态弹窗
    /// </summary>
    /// <typeparam name="TView">窗口类型</typeparam>
    /// <typeparam name="TViewModel">窗口绑定的视图模型</typeparam>
    /// <param name="parameter">参数</param>
    /// <param name="callback">弹窗确认后的处理函数</param>
    /// <returns></returns>
    Task ShowDialogAsync<TView, TViewModel>(object? parameter = null, Action<TViewModel>? callback = null)
        where TView : Window, IWithParameterWindow
        where TViewModel : class;
}

/// <summary>
/// 带参窗口接口
/// </summary>
public interface IWithParameterWindow
{
    /// <summary>
    /// 根据参数进行完成初始化
    /// </summary>
    /// <param name="parameter"></param>
    Task InitializeAsync(object? parameter);
}
