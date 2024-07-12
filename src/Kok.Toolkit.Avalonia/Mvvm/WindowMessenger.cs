using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;

namespace Kok.Toolkit.Avalonia.Mvvm;

/// <summary>
/// 窗体消息信使
/// </summary>
public static class WindowMessenger
{
    /// <summary>
    /// 为指定窗体注册某类消息
    /// </summary>
    /// <typeparam name="TSender">消息发送者类型</typeparam>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <param name="recipient">消息接收者</param>
    /// <param name="onGotMessage">接收到消息后的处理</param>
    public static void Register<TSender, TMessage>(Window recipient, Action<TMessage> onGotMessage) where TSender : class where TMessage : WindowMessage
        => WeakReferenceMessenger.Default.Register<TMessage, string>(recipient, typeof(TSender).ToString(), (_, m) => onGotMessage(m));

    /// <summary>
    /// 为指定窗体取消某类消息
    /// </summary>
    /// <typeparam name="TSender">消息发送者类型</typeparam>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <param name="recipient">消息接收者</param>
    public static void UnRegister<TSender, TMessage>(Window recipient) where TSender : class where TMessage : WindowMessage
        => WeakReferenceMessenger.Default.Unregister<TMessage, string>(recipient, typeof(TSender).ToString());

    /// <summary>
    /// 响应窗体关闭消息
    /// </summary>
    /// <param name="recipient">消息接收者</param>
    public static void ResponseCloseWinMessage<TSender>(Window recipient) where TSender : class
    {
        async void OnGotMessage(CloseWindowMessage _)
            => await Dispatcher.UIThread.InvokeAsync(recipient.Close);

        Register<TSender, CloseWindowMessage>(recipient, OnGotMessage);
        recipient.Closed += (_, _) => UnRegister<TSender, CloseWindowMessage>(recipient);
    }

    /// <summary>
    /// 响应UI通知消息
    /// </summary>
    /// <typeparam name="TSender">消息发送者类型</typeparam>
    /// <param name="recipient">消息接收者</param>
    /// <param name="onGotMessage">接收到消息后的处理</param>
    public static void ResponseNotificationMessage<TSender>(Window recipient, Action<NotificationMessage> onGotMessage) where TSender : class
    {
        async void OnGotMessage(NotificationMessage message)
            => await Dispatcher.UIThread.InvokeAsync(() => onGotMessage(message));
        Register<TSender, NotificationMessage>(recipient, onGotMessage);
        recipient.Closed += (_, _) => UnRegister<TSender, NotificationMessage>(recipient);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="sender">消息发送者</param>
    /// <param name="message">发送的内容</param>
    public static void Send<T>(object sender, T message) where T : WindowMessage
        => WeakReferenceMessenger.Default.Send(message, sender.GetType().ToString());

    /// <summary>
    /// 发送关闭窗体消息
    /// </summary>
    /// <param name="sender">消息发送者</param>
    public static void SendCloseWinMessage(object sender)
        => WeakReferenceMessenger.Default.Send(new CloseWindowMessage(sender), sender.GetType().ToString());
}
