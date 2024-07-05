using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kok.Toolkit.Avalonia.Mvvm;
using System;

namespace Kok.Test.AvaloniaDemo.ViewModels;

public partial class FirstViewModel : MessengerViewModel
{
    [ObservableProperty]
    private string _title = "First View";

    [RelayCommand]
    public void CloseWindow()
    {
        WindowMessenger.Send(this, new CloseWindowMessage(new WeakReference(this)));
    }
}
