using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Kok.Test.AvaloniaDemo.ViewModels;

namespace Kok.Test.AvaloniaDemo.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<MainWindowViewModel>();
        }
    }
}
