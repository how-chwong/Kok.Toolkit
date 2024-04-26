using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Kok.Test.WpfDemo.Views;
using Kok.Toolkit.Wpf.Mvvm;

namespace Kok.Test.WpfDemo.ViewModels;

public partial class MainViewModel : MessengerViewModel<AlarmMessage>
{
    [ObservableProperty]
    private string? _statusMessage;

    protected override void OnGotMessage(object receiver, AlarmMessage message)
    {
        StatusMessage = message.Message;
    }

    [ObservableProperty]
    private MessageViewModel _otherModel;

    public MainViewModel()
    {
        OtherModel = new MessageViewModel();
    }

    [RelayCommand]
    private void ShowLogWin()
    {
        var win = Ioc.Default.GetService<LogMonitorView>();
        win.Show();
    }
}
