using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kok.Toolkit.Wpf.Mvvm;
using Kok.Toolkit.Wpf.Navigation;
using System.Diagnostics;
using System.Windows.Threading;

namespace Kok.Test.WpfDemo.ViewModels;

public partial class LogMonitorViewModel : MessengerViewModel, IConfirmNavigation
{
    [ObservableProperty]
    private string? _field;

    [ObservableProperty]
    private string? _content;

    [ObservableProperty]
    private bool _hasAlarm;

    partial void OnHasAlarmChanged(bool value)
    {
        if (value)
            SendMessage(new AlarmMessage($"{DateTime.Now:HH:mm:ss.fff} {Field} has alarm!@{Content}"));
    }

    private readonly DispatcherTimer _timer = new();

    [RelayCommand]
    private void ClearData() => _timer.Stop();

    private readonly int _cycle;

    public LogMonitorViewModel()
    {
        _cycle = new Random().Next(1, 5);
        Field = $"工作状态{Thread.CurrentThread.ManagedThreadId}";
        Content = _cycle.ToString();
        _timer.Interval = TimeSpan.FromMilliseconds(10);
        _timer.Tick += UpdateLog;
        _timer.Start();
    }

    private void UpdateLog(object? sender, EventArgs e)
    {
        HasAlarm = DateTime.Now.Second % _cycle == 0;
        Debug.Print($"{_cycle}:运行一次@{DateTime.Now:HH:mm:ss.fff}");
    }

    public bool OnNavigateTo()
    {
        return true;
    }

    public bool OnNavigateFrom()
    {
        _timer.Stop();
        SendMessage(new AlarmMessage($"{DateTime.Now:HH:mm:ss.fff} {Field} has exited!@{Content}"));

        return true;
    }
}

public class AlarmMessage
{
    public DateTime Time { get; set; }
    public string? Message { get; set; }

    public AlarmMessage(string message)
    {
        Time = DateTime.Now;
        Message = message;
    }
}
