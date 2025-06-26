using Kok.Toolkit.Core.Log;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Kok.Toolkit.Core.Timers;

/// <summary>
/// 多媒体定时器，仅在Windows平台上使用
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class MultimediaTimer : ITimer
{
    // Win32 API 声明
    [DllImport("winmm.dll", SetLastError = true)]
    private static extern uint timeSetEvent(
        uint msDelay,
        uint msResolution,
        TimerCallback handler,
        IntPtr userCtx,
        uint eventType);

    [DllImport("winmm.dll", SetLastError = true)]
    private static extern uint timeKillEvent(uint timerId);

    private delegate void TimerCallback(uint id, uint msg, IntPtr user, IntPtr param1, IntPtr param2);

    // 内部状态
    private uint _timerId;

    private readonly TimerCallback _nativeCallback;
    private readonly Action<object?> _userCallback;
    private readonly object? _state;
    private volatile bool _disposed;

    /// <summary>
    /// 初始化多媒体定时器
    /// </summary>
    /// <param name="callback">定时器回调方法</param>
    /// <param name="state">传递给回调的用户状态</param>
    /// <param name="period">执行间隔(毫秒)</param>
    public MultimediaTimer(Action<object?> callback, object? state, int period)
    {
        _userCallback = callback;
        _state = state;
        _nativeCallback = TimerHandler;

        Change(period);
    }

    /// <summary>
    /// 修改定时器设置
    /// </summary>
    public void Change(int period)
    {
        if (_disposed) return;
        if (_timerId != 0)
        {
            timeKillEvent(_timerId);
            _timerId = 0;
        }
        if (period < 0) return;
        // 启动新定时器 (0 = 单次执行, 1 = 周期执行)
        uint eventType = period <= 0 ? 0u : 1u;

        _timerId = timeSetEvent(
            msDelay: (uint)Math.Max(1, period),
            msResolution: 1,
            handler: _nativeCallback,
            userCtx: IntPtr.Zero,
            eventType: eventType);

        if (_timerId == 0)
        {
            Tracker.WriteError($"创建定时器失败:{Marshal.GetLastWin32Error()}");
        }
    }

    private void TimerHandler(uint id, uint msg, IntPtr user, IntPtr param1, IntPtr param2)
    {
        try
        {
            _userCallback?.Invoke(_state);
        }
        catch (Exception ex)
        {
            Tracker.WriteError($"媒体定时器执行失败:{ex}");
        }
    }

    /// <summary>
    /// 停止定时器并释放资源
    /// </summary>
    public void Stop()
    {
        if (_disposed) return;

        if (_timerId != 0)
        {
            timeKillEvent(_timerId);
            _timerId = 0;
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        Stop();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 析构函数，确保资源被释放
    /// </summary>
    ~MultimediaTimer() => Dispose();
}
