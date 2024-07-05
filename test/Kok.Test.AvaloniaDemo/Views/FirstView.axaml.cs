using Avalonia.Controls;
using Kok.Test.AvaloniaDemo.ViewModels;
using Kok.Toolkit.Avalonia.Mvvm;

namespace Kok.Test.AvaloniaDemo.Views;

public partial class FirstView : Window
{
    public FirstView()
    {
        InitializeComponent();
        WindowMessenger.Register<FirstViewModel, CloseWindowMessage>(this, m =>
        {
            // WindowMessenger.UnRegister<FirstViewModel, CloseWindowMessage>(this);
            this.Close(true);
        });
        
        Closed += (o, e) => WindowMessenger.UnRegister<FirstViewModel, CloseWindowMessage>(this);
    }
}
