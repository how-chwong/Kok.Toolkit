using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kok.Toolkit.Wpf.Mvvm;

namespace Kok.Test.WpfDemo.ViewModels
{
    public partial class CallbackDialogViewModel : ViewModel
    {
        [ObservableProperty]
        private bool? _dialogResult;

        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private string? _sayHello;

        [RelayCommand]
        private void MakeHello()
        {
            SayHello = $"Hello,{Name}";
            DialogResult = true;
        }

        [RelayCommand]
        private void Close()
        {
            DialogResult = false;
        }
    }
}
