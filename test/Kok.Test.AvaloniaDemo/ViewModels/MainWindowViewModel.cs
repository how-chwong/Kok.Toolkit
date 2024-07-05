using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kok.Toolkit.Avalonia.Dialogs;
using Kok.Toolkit.Avalonia.Mvvm;
using Kok.Toolkit.Avalonia.Navigation;

namespace Kok.Test.AvaloniaDemo.ViewModels
{
    public partial class MainWindowViewModel : ViewModel
    {
        private readonly INavigationService _navigation;
        private readonly IDialogService _dialogs;

        public MainWindowViewModel(INavigationService navigation, IDialogService dialogs)
        {
            _navigation = navigation;
            _dialogs = dialogs;
        }

        [ObservableProperty]
        private string greeting = "Welcome to Avalonia!";

        [RelayCommand]
        private void OpenFirstWin()
        {
            _dialogs.Show<Views.FirstView>();
        }
    }
}
