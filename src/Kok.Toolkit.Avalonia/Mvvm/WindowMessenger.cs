using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;

namespace Kok.Toolkit.Avalonia.Mvvm;

public record WindowMessage(WeakReference Sender);

public record CloseWindowMessage(WeakReference Sender) : WindowMessage(Sender);

public static class WindowMessenger
{
    public static void Register<TSender, TMessage>(Window recipient, Action<TMessage> onGotMessage) where TSender : class where TMessage : WindowMessage
    {
        WeakReferenceMessenger.Default.Register<TMessage, string>(recipient, typeof(TSender).ToString(), (_, m) => onGotMessage(m));
    }

    public static void UnRegister<TSender, TMessage>(Window recipient) where TSender : class where TMessage : WindowMessage
    {
        WeakReferenceMessenger.Default.Unregister<TMessage, string>(recipient, typeof(TSender).ToString());
    }

    public static void Send<T>(object sender, T message) where T : WindowMessage
    {
        WeakReferenceMessenger.Default.Send(message, sender.GetType().ToString());
    }
}
