using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kok.Toolkit.Wpf.Mvvm;
using System.Windows.Threading;

namespace Kok.Test.WpfDemo.ViewModels;

public partial class LogMonitorViewModel : MessengerViewModel
{
    [ObservableProperty]
    private string? _field;

    [ObservableProperty]
    private string? _content;

    [ObservableProperty]
    private bool _hasAlarm;

    partial void OnHasAlarmChanged(bool value)
    {
        if (value) SendMessage(new AlarmMessage($"{DateTime.Now:HH:mm:ss.fff} {Field} has alarm!@{Content}"));
    }

    private readonly DispatcherTimer _timer = new();

    [RelayCommand]
    private void ClearData() => _timer.Stop();

    private readonly int _cycle = new Random().Next(1, 5);

    public LogMonitorViewModel()
    {
        Field = "工作状态";
        Content = _cycle.ToString();
        _timer.Interval = TimeSpan.FromMilliseconds(50);
        _timer.Tick += UpdateLog;
        _timer.Start();
    }

    private void UpdateLog(object? sender, EventArgs e)
    {
        HasAlarm = DateTime.Now.Second % _cycle == 0;
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
