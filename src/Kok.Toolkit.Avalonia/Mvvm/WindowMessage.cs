namespace Kok.Toolkit.Avalonia.Mvvm;

/// <summary>
/// 窗体消息基类
/// </summary>
/// <param name="Sender"></param>
public abstract record WindowMessage(WeakReference Sender);

/// <summary>
/// 关闭窗体的消息
/// </summary>
/// <param name="Sender"></param>
public record CloseWindowMessage(WeakReference Sender) : WindowMessage(Sender)
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="sender"></param>
    public CloseWindowMessage(object sender) : this(new WeakReference(sender))
    {
    }
}

/// <summary>
/// UI通知消息
/// </summary>
/// <param name="Sender"></param>
/// <param name="Content"></param>
/// <param name="IsWarning"></param>
public record NotificationMessage(WeakReference Sender, string Content, bool IsWarning) : WindowMessage(Sender)
{
    public NotificationMessage(object sender, string content, bool isWarning) : this(new WeakReference(sender), content, isWarning)
    {
    }
}
