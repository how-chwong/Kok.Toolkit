using CommunityToolkit.Mvvm.DependencyInjection;
using Kok.Test.WpfDemo.ViewModels;
using System.Windows.Controls;

namespace Kok.Test.WpfDemo.Views
{
    /// <summary>
    /// LogMonitorView.xaml 的交互逻辑
    /// </summary>
    public partial class LogMonitorView : UserControl
    {
        public LogMonitorView()
        {
            InitializeComponent();
            //DataContext = Ioc.Default.GetService<LogMonitorViewModel>();
        }
    }
}
