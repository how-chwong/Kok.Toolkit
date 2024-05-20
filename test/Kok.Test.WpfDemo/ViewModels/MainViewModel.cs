using CommunityToolkit.Mvvm.ComponentModel;
using Kok.Test.WpfDemo.Models;
using Kok.Test.WpfDemo.Views;
using Kok.Toolkit.Wpf.Mvvm;
using Kok.Toolkit.Wpf.Navigation;
using System.Collections.ObjectModel;

namespace Kok.Test.WpfDemo.ViewModels;

public partial class MainViewModel : MessengerViewModel<AlarmMessage>
{
    [ObservableProperty]
    private ObservableCollection<MenuItem> _menus;

    [ObservableProperty]
    private MenuItem? _selectedItem;

    partial void OnSelectedItemChanged(MenuItem? value)
    {
        if (value == null) return;
        switch (value.type)
        {
            case ViewType.Dialog:
                Navigation.ToView<DialogDemoView>();
                break;

            case ViewType.Message:
                Navigation.ToView<LogMonitorView>();//<LogMonitorViewModel>();
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
