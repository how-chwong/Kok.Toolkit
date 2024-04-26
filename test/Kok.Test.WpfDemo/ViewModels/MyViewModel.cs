using CommunityToolkit.Mvvm.Input;
using Kok.Toolkit.Wpf.Mvvm;
using System.Windows;
using System.Windows.Input;

namespace Kok.Test.WpfDemo.ViewModels;

public class MyViewModel : ViewModel
{
    private string? _name;

    public string? Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public ICommand SayHelloCommand { get; }

    private void SayHello()
    {
        MessageBox.Show($"hello,{Name}");
    }

    public MyViewModel()
    {
        SayHelloCommand = new RelayCommand(SayHello);
    }
}
