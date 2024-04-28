using CommunityToolkit.Mvvm.DependencyInjection;
using Kok.Test.WpfDemo.ViewModels;
using Kok.Toolkit.Wpf.Dialogs;
using System.Windows;

namespace Kok.Test.WpfDemo.Views;

/// <summary>
/// ParameterView.xaml 的交互逻辑
/// </summary>
public partial class ParameterView : Window, IWithParameterWindow
{
    public ParameterView()
    {
        InitializeComponent();
    }

    public Task InitializeAsync(object? parameter)
    {
        var temp = Ioc.Default.GetService<ParameterViewModel>();
        temp!.Parameter = parameter?.ToString();
        DataContext = temp;
        return Task.CompletedTask;
    }
}
