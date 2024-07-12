using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;

namespace Kok.Toolkit.Avalonia.Mvvm;

/// <summary>
/// 窗体消息基类
/// </summary>
/// <param name="Sender"></param>
public record WindowMessage(WeakReference Sender);

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
/// 窗体消息信使
/// </summary>
public static class WindowMessenger
{
    /// <summary>
    /// 响应窗体关闭消息
    /// </summary>
    /// <param name="recipient"></param>
    public static void ResponseCloseWinMessage<TSender>(Window recipient) where TSender : class
    {
        async void OnGotMessage(CloseWindowMessage _)
            => await Dispatcher.UIThread.InvokeAsync(recipient.Close);

        Register<TSender, CloseWindowMessage>(recipient, OnGotMessage);
        recipient.Closed += (_, _) => UnRegister<TSender, CloseWindowMessage>(recipient);
    }

    /// <summary>
    /// 为指定窗体注册某类消息
    /// </summary>
    /// <typeparam name="TSender"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="recipient"></param>
    /// <param name="onGotMessage"></param>
    public static void Register<TSender, TMessage>(Window recipient, Action<TMessage> onGotMessage) where TSender : class where TMessage : WindowMessage
        => WeakReferenceMessenger.Default.Register<TMessage, string>(recipient, typeof(TSender).ToString(), (_, m) => onGotMessage(m));

    /// <summary>
    /// 为指定窗体取消某类消息
    /// </summary>
    /// <typeparam name="TSender"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="recipient"></param>
    public static void UnRegister<TSender, TMessage>(Window recipient) where TSender : class where TMessage : WindowMessage
        => WeakReferenceMessenger.Default.Unregister<TMessage, string>(recipient, typeof(TSender).ToString());

    public static void Send<T>(object sender, T message) where T : WindowMessage
        => WeakReferenceMessenger.Default.Send(message, sender.GetType().ToString());

    /// <summary>
    /// 发送关闭窗体消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    public static void SendCloseWinMessage(object sender)
        => WeakReferenceMessenger.Default.Send(new CloseWindowMessage(sender), sender.GetType().ToString());
}
