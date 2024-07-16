using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Kok.Test.AvaloniaDemo.ViewModels;
using Kok.Toolkit.Avalonia.Mvvm;
using System;

namespace Kok.Test.AvaloniaDemo.Views;

public partial class FirstView : Window
{
    private WindowNotificationManager? _notificationManager;

    public FirstView()
    {
        InitializeComponent();
        WindowMessenger.ResponseCloseWinMessageWithConfirm<FirstViewModel>(this);
        WindowMessenger.ResponseNotificationMessage<FirstViewModel>(this, OnGotNotification);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _notificationManager = new WindowNotificationManager(this) { MaxItems = 10, Position = NotificationPosition.TopLeft };
    }

    private void OnGotNotification(NotificationMessage message)
        => _notificationManager?.Show(new Notification("",
            message.Content,
            message.IsWarning
                ? NotificationType.Warning
                : NotificationType.Success));
}
