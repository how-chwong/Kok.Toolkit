using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.DependencyInjection;
using Kok.Test.AvaloniaDemo.ViewModels;
using Kok.Toolkit.Avalonia.Mvvm;

namespace Kok.Test.AvaloniaDemo.Views
{
    public partial class MainWindow : Window
    {
        private WindowNotificationManager? _notificationManager;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = Ioc.Default.GetService<MainWindowViewModel>();
            WindowMessenger.ResponseNotificationMessage<FirstViewModel>(this, OnGotNotification);
            WindowMessenger.ResponseNotificationMessage<MainWindowViewModel>(this, OnGotNotification);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _notificationManager = new WindowNotificationManager(this) { MaxItems = 10, Position = NotificationPosition.BottomRight };
        }

        private void OnGotNotification(NotificationMessage message)
            => _notificationManager?.Show(new Notification("",
                message.Content,
                message.IsWarning
                    ? NotificationType.Warning
                    : NotificationType.Success));
    }
}
