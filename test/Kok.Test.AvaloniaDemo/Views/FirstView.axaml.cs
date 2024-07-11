using Avalonia.Controls;
using Kok.Test.AvaloniaDemo.ViewModels;
using Kok.Toolkit.Avalonia.Mvvm;

namespace Kok.Test.AvaloniaDemo.Views;

public partial class FirstView : Window
{
    public FirstView()
    {
        InitializeComponent();
        WindowMessenger.ResponseCloseWinMessage<FirstViewModel>(this);
    }
}
