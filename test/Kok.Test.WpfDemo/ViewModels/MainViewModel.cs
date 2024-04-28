using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Kok.Test.WpfDemo.Views;
using Kok.Toolkit.Wpf.Dialogs;
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

    private readonly IDialogService _dialogs;

    public MainViewModel(IDialogService dialogs)
    {
        _dialogs = dialogs;
        OtherModel = new MessageViewModel();
    }

    [RelayCommand]
    private void ShowLogWin() => _dialogs.Show<LogMonitorView>();

    #region 带参数弹窗

    [ObservableProperty]
    private string? _text;

    [RelayCommand]
    private async void ShowParameterWin(object? args) => await _dialogs.ShowDialogAsync<ParameterView>(args);

    #endregion 带参数弹窗
}
