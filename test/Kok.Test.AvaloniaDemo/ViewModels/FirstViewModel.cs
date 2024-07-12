using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kok.Toolkit.Avalonia.Mvvm;

namespace Kok.Test.AvaloniaDemo.ViewModels;

public partial class FirstViewModel : MessengerViewModel, IViewModelNotifiable
{
    [ObservableProperty]
    private string _title = "First View";

    [RelayCommand]
    private void CloseWindow() => WindowMessenger.SendCloseWinMessage(this);

    [RelayCommand]
    private void Send()
        => (this as IViewModelNotifiable).SendNotification("错误", "发生了一个错误", NotificationType.Error);

    public WindowNotificationManager? NotificationManager { get; set; }
}
