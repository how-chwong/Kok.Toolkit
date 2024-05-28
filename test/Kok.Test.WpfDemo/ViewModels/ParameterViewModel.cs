using CommunityToolkit.Mvvm.ComponentModel;
using Kok.Toolkit.Wpf.Mvvm;

namespace Kok.Test.WpfDemo.ViewModels;

public partial class ParameterViewModel : ViewModel
{
    [ObservableProperty]
    private string? _parameter;
}
