using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Kok.Toolkit.Avalonia.Mvvm;

/// <summary>
/// 视图模型基类
/// </summary>
public abstract class ViewModel : ObservableObject
{
}

/// <summary>
/// 可广播消息的视图模型
/// </summary>
public class MessengerViewModel : ObservableRecipient
{
    /// <summary>
    /// 构造一个可广播消息的视图模型
    /// 默认不支持接收消息
    /// </summary>
    protected MessengerViewModel()
    {
        IsActive = false;
    }

    /// <summary>
    /// 广播消息
    /// </summary>
    /// <typeparam name="T">广播的消息类型</typeparam>
    /// <param name="message">广播的消息实例</param>
    protected void SendMessage<T>(T message) where T : class
        => Messenger.Send(message, nameof(T));
}

/// <summary>
/// 订阅消息T的视图模型
/// </summary>
/// <typeparam name="T">可接收的消息类型</typeparam>
public abstract class MessengerViewModel<T> : MessengerViewModel where T : class
{
    /// <summary>
    /// 构造一个订阅了消息的视图模型
    /// </summary>
    protected MessengerViewModel()
    {
        IsActive = true;
    }

    ///<inheritdoc />
    protected override void OnActivated()
        => Messenger.Register<MessengerViewModel, T, string>(this, nameof(T), OnGotMessage);

    /// <summary>
    /// 收到消息后的处理逻辑
    /// </summary>
    /// <param name="receiver">消息来源</param>
    /// <param name="message">收到的消息实例</param>
    protected abstract void OnGotMessage(object receiver, T message);
}

/// <summary>
/// 可发送通知的视图模型接口
/// </summary>
public interface IViewModelNotifiable
{
    /// <summary>
    /// 通知管理器
    /// </summary>
    WindowNotificationManager? NotificationManager { get; protected set; }

    /// <summary>
    /// 设置通知的显示级别
    /// </summary>
    /// <param name="window"></param>
    /// <param name="maxItems"></param>
    /// <param name="position"></param>
    void SetNotifyTopLevel(Window window, int maxItems = 10, NotificationPosition position = NotificationPosition.TopRight)
    {
        NotificationManager = new WindowNotificationManager(window) { MaxItems = maxItems, Position = position };
    }

    /// <summary>
    /// 发送一条通知
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="message">通知内容</param>
    /// <param name="type">类型</param>
    /// <param name="seconds">显示时间，单位秒</param>
    void SendNotification(string title, string message, NotificationType type, int seconds = 5)
        => NotificationManager?.Show(new Notification(title, message, type, TimeSpan.FromSeconds(seconds)));
}
