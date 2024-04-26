using CommunityToolkit.Mvvm.ComponentModel;
using Kok.Toolkit.Wpf.Mvvm;

namespace Kok.Test.WpfDemo.ViewModels;

public partial class MessageViewModel : MessengerViewModel<AlarmMessage>
{
    [ObservableProperty]
    private string _message;

    protected override void OnGotMessage(object receiver, AlarmMessage message)
    {
        Message = $"{message.Time} @{message.Message}";
    }
}
