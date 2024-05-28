using CommunityToolkit.Mvvm.DependencyInjection;
using Kok.Test.WpfDemo.ViewModels;
using System.Windows;

namespace Kok.Test.WpfDemo.Views
{
    /// <summary>
    /// SecondView.xaml 的交互逻辑
    /// </summary>
    public partial class SecondView : Window
    {
        public SecondView()
        {
            DataContext = Ioc.Default.GetService<SecondModel>();
            InitializeComponent();
        }
    }
}
