using CommunityToolkit.Mvvm.ComponentModel;
using Kok.Test.WpfDemo.Models;
using Kok.Toolkit.Wpf.Mvvm;
using Kok.Toolkit.Wpf.Navigation;
using System.Collections.ObjectModel;

namespace Kok.Test.WpfDemo.ViewModels;

public partial class MainViewModel : MessengerViewModel<AlarmMessage>
{
    [ObservableProperty]
    public ObservableCollection<MenuItem> _menus;

    [ObservableProperty]
    private MenuItem? _selectedItem;

    partial void OnSelectedItemChanged(MenuItem? value)
    {
        if (value == null) return;
        switch (value.type)
        {
            case ViewType.Dialog:
                Navigation.NavigateTo<DialogDemoViewModel>();
                break;

            case ViewType.Message:
                Navigation.NavigateTo("LogMonitorView");
                break;
        }
    }

    [ObservableProperty]
    private string? _statusMessage;

    protected override void OnGotMessage(object receiver, AlarmMessage message)
    {
        StatusMessage = message.Message;
    }

    [ObservableProperty]
    private INavigationService _navigation;

    public MainViewModel(INavigationService navigation)
    {
        Navigation = navigation;
        Menus = new ObservableCollection<MenuItem>
        {
            new("对话框",ViewType.Dialog),
            new("消息",ViewType.Message)
        };
    }
}
