using CommunityToolkit.Mvvm.DependencyInjection;
using Kok.Test.WpfDemo.ViewModels;
using System.Windows;

namespace Kok.Test.WpfDemo.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<MainViewModel>();
        }
    }
}
