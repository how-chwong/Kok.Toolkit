using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kok.Toolkit.Avalonia.Mvvm;

namespace Kok.Test.AvaloniaDemo.ViewModels;

public partial class FirstViewModel : MessengerViewModel
{
    [ObservableProperty]
    private string _title = "First View";

    [RelayCommand]
    private void CloseWindow() => WindowMessenger.SendCloseWinMessage(this);

    [RelayCommand]
    private void Send()
        => WindowMessenger.Send(this, new NotificationMessage(this, "发生了一个错误", true));

    [RelayCommand]
    private void VmSend()
        => SendMessage(new NotificationMessage(this, "这是VM发送的消息", false));
}
