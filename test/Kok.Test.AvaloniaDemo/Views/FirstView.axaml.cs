using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Kok.Test.AvaloniaDemo.ViewModels;
using Kok.Toolkit.Avalonia.Mvvm;

namespace Kok.Test.AvaloniaDemo.Views;

public partial class FirstView : Window
{
    public FirstView()
    {
        InitializeComponent();
        WindowMessenger.ResponseCloseWinMessage<FirstViewModel>(this);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (DataContext is IViewModelNotifiable vm) vm.SetNotifyTopLevel(this, 10, NotificationPosition.BottomLeft);
    }
}
