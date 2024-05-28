using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kok.Test.WpfDemo.Services;
using Kok.Toolkit.Wpf.Mvvm;
using System.Windows;

namespace Kok.Test.WpfDemo.ViewModels;

public partial class SecondModel : ViewModel
{
    private readonly IMyService _service;

    public SecondModel(IMyService service)
    {
        _service = service;
    }

    [ObservableProperty]
    private string? _name;

    [RelayCommand]
    private void SayHello()
    {
        MessageBox.Show(_service.SayHello(_name ?? "vm"));
    }
}
