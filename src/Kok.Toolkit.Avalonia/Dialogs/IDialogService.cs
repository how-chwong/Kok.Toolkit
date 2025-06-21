using Avalonia.Controls;

namespace Kok.Toolkit.Avalonia.Dialogs;

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
    /// 非模态带参弹窗
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameter"></param>
    void Show<T>(object parameter) where T : Window, IWithParameterWindow;

    /// <summary>
    /// 模态弹窗
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<bool?> ShowDialogAsync<T>() where T : Window;

    /// <summary>
    /// 模态弹窗
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task ShowDialogAsync<T>(Action<object>? callback) where T : Window;

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
    /// <typeparam name="T"></typeparam>
    /// <param name="parameter"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    Task ShowDialogAsync<T>(object? parameter, Action<object>? callback)
        where T : Window, IWithParameterWindow;
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
