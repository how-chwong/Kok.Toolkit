using CommunityToolkit.Mvvm.ComponentModel;
using Kok.Toolkit.Wpf.Mvvm;

namespace Kok.Test.WpfDemo.ViewModels;

public partial class MenuItemViewModel : ViewModel
{
    [ObservableProperty]
    private string? _title;

    public MenuItemViewModel(string title)
    {
        Title = title;
    }
}
