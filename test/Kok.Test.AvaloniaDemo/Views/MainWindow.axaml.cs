using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.DependencyInjection;
using Kok.Test.AvaloniaDemo.ViewModels;
using Kok.Toolkit.Avalonia.Mvvm;

namespace Kok.Test.AvaloniaDemo.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<MainWindowViewModel>();
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            if (DataContext is IViewModelNotifiable vm)
                vm.SetNotifyTopLevel(this);
        }
    }
}
