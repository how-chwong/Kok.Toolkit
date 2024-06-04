using System.Windows;

namespace Kok.Toolkit.Wpf.Dialogs;

/// <summary>
/// 对话框关闭器
/// </summary>
public static class DialogCloser
{
    /// <summary>
    /// 对话框结果属性
    /// </summary>
    public static readonly DependencyProperty DialogResultProperty =
        DependencyProperty.RegisterAttached(
            "DialogResult",
            typeof(bool?),
            typeof(DialogCloser),
            new PropertyMetadata(DialogResultChanged));

    private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Window window)
            window.DialogResult = e.NewValue as bool?;
    }

    /// <summary>
    /// 设置对话框结果
    /// </summary>
    /// <param name="target">对话窗口</param>
    /// <param name="value">结果</param>
    public static void SetDialogResult(Window target, bool? value)
    {
        target.SetValue(DialogResultProperty, value);
    }
}
