using Kok.Toolkit.Core.Log;

namespace Kok.Toolkit.Core;

/// <summary>
/// 防重入定时器
/// </summary>
public class AntiReTimer
{
    /// <summary>
    /// 定时器
    /// </summary>
    private readonly Timer _timer;

    /// <summary>
    /// 定时器的执行周期，单位毫秒
    /// </summary>
    private readonly int _period;

    /// <summary>
    /// 周期执行的方法
    /// </summary>
    private readonly Action<object?> _action;

    /// <summary>
    /// 判断是否重置计数器，即将计数器设置为0
    /// </summary>
    private readonly Func<bool>? _isResetCounter;

    /// <summary>
    /// 防重入锁
    /// </summary>
    private int _reentryLock;

    /// <summary>
    /// 执行周期数
    /// 0：无限循环执行；>0：仅执行指定周期数
    /// </summary>
    private int _runTimes = 5;

    /// <summary>
    /// 构造一个无限循环执行的定时器
    /// </summary>
    /// <param name="action">执行方法</param>
    /// <param name="state">执行方法参数</param>
    /// <param name="dueTime">定时器延迟启动毫秒数</param>
    /// <param name="period">定时器执行间隔毫秒数</param>
    public AntiReTimer(Action<object?> action, object state, int dueTime = 5000, int period = 1000)
    {
        _action = action;
        _isResetCounter = null;
        _period = period;
        _timer = new Timer(DoWork, state, dueTime, _period);
    }

    /// <summary>
    /// 构造一个执行固定周期数的定时器
    /// </summary>
    /// <param name="isResetCounter">返回值标识是否重新开始计数,true:结束当前计数，从1开始计数；false:当前计数加1</param>
    /// // <param name="action">执行方法</param>
    /// <param name="state">执行方法参数</param>
    /// <param name="dueTime">定时器延迟启动毫秒数</param>
    /// <param name="period">定时器执行间隔毫秒数</param>
    /// <param name="runTimes">定时器执行最大周期数</param>
    public AntiReTimer(Func<bool> isResetCounter, Action<object?> action, object state, int dueTime = 5000, int period = 1000, int runTimes = 0)
    {
        _isResetCounter = isResetCounter;
        _action = action;
        _period = period;
        _timer = new Timer(DoWork, state, dueTime, _period);
        _runTimes = runTimes;
    }

    //执行周期计数器
    private int _counter;

    private long _lastExecuteTime;

    private void DoWork(object? state)
    {
        if (Interlocked.Exchange(ref _reentryLock, 1) != 0)
        {
            if (DateTime.Now.Ticks - _lastExecuteTime >= _period * TimeSpan.TicksPerMillisecond)
            {
                Tracker.WriteWarn($"定时任务未获取到锁，距上次执行超过了间隔周期：{Thread.CurrentThread.ManagedThreadId}-{_action.Method.Name}");
            }
            else
            {
                Tracker.WriteWarn($"定时任务{Thread.CurrentThread.ManagedThreadId}单次执行时长超过了间隔周期：{_action.Method.Name}");
                return;
            }
        }

        try
        {
            var r = _isResetCounter?.Invoke();
            _counter = _isResetCounter != null && r == true ? 0 : _counter + 1;
            if (_runTimes != 0 && _counter >= _runTimes) return;
            _action(state);
        }
        catch (Exception ex)
        {
            Tracker.WriteError($"定时任务执行失败：{ex}");
        }
        finally
        {
            _lastExecuteTime = DateTime.Now.Ticks;
            Interlocked.Exchange(ref _reentryLock, 0);
        }
    }

    /// <summary>
    /// 设置定时器循环间隔
    /// </summary>
    /// <param name="dueTime">定时器延迟启动时间，单位毫秒，默认1000</param>
    /// <param name="period">定时器执行间隔，单位毫秒，默认100</param>
    public void SetInterval(int dueTime = 1000, int period = 1000) => _timer.Change(dueTime, period);

    /// <summary>
    /// 设置执行周期数
    /// </summary>
    /// <param name="times"></param>
    public void SetRunTimes(int times) => _runTimes = times;

    /// <summary>
    /// 停止定时器
    /// </summary>
    public void Stop()
    {
        _timer.Change(-1, int.MaxValue);
        _timer.Dispose();
    }

    /// <summary>
    /// 析构
    /// </summary>
    ~AntiReTimer() => Stop();
}
