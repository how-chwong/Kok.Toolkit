using Kok.Test.WpfDemo.ViewModels;
using Kok.Toolkit.Wpf.Dialogs;
using System.Windows;

namespace Kok.Test.WpfDemo.Views
{
    /// <summary>
    /// CallbackDialogView.xaml 的交互逻辑
    /// </summary>
    public partial class CallbackDialogView : Window, IWithParameterWindow
    {
        public CallbackDialogView()
        {
            InitializeComponent();
        }

        public Task InitializeAsync(object? parameter)
        {
            if (parameter == null) return Task.CompletedTask;

            var dc = new CallbackDialogViewModel();
            dc.Name = parameter.ToString();
            DataContext = dc;

            return Task.CompletedTask;
        }
    }
}
