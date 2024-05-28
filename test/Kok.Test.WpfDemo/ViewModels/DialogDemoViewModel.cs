using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kok.Test.WpfDemo.Views;
using Kok.Toolkit.Wpf.Dialogs;
using Kok.Toolkit.Wpf.Mvvm;

namespace Kok.Test.WpfDemo.ViewModels;

public partial class DialogDemoViewModel : ViewModel
{
    private readonly IDialogService _dialogs;

    public DialogDemoViewModel(IDialogService dialogs)
    {
        _dialogs = dialogs;
    }

    #region 非模态

    [RelayCommand]
    private void ShowDialog() => _dialogs.Show<FirstView>();

    #endregion 非模态

    #region 仅弹窗

    [RelayCommand]
    private async void ShowDialog1() => await _dialogs.ShowDialogAsync<Dialog1View>();

    #endregion 仅弹窗

    #region 带参数弹窗

    [ObservableProperty]
    private string? _text;

    [RelayCommand]
    private async void ShowParameterWin(object? args) => await _dialogs.ShowDialogAsync<ParameterView>(args);

    #endregion 带参数弹窗

    #region 带参及回调处理的弹窗

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _helloText;

    [RelayCommand]
    private async void ShowCallbackWin(object? args) =>
        await _dialogs.ShowDialogAsync<CallbackDialogView>(args, OnDialogClosed);

    private void OnDialogClosed(object obj)
    {
        if (obj is CallbackDialogViewModel model)
        {
            HelloText = model.SayHello;
        }
    }

    #endregion 带参及回调处理的弹窗
}
