using CommunityToolkit.Mvvm.DependencyInjection;
using Kok.Test.WpfDemo.ViewModels;
using System.Windows.Controls;

namespace Kok.Test.WpfDemo.Views
{
    /// <summary>
    /// DialogDemoView.xaml 的交互逻辑
    /// </summary>
    public partial class DialogDemoView : UserControl
    {
        public DialogDemoView()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<DialogDemoViewModel>();
        }
    }
}
