using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kok.Test.WpfDemo.Views;
using Kok.Toolkit.Wpf.Dialogs;
using Kok.Toolkit.Wpf.Mvvm;

namespace Kok.Test.WpfDemo.ViewModels;

internal partial class DialogDemoViewModel : ViewModel
{
    private readonly IDialogService _dialogs;

    public DialogDemoViewModel(IDialogService dialogs)
    {
        _dialogs = dialogs;
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
