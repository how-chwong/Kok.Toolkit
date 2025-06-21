namespace Kok.Toolkit.Core.Timers;

/// <summary>
/// 定时器接口
/// </summary>
public interface ITimer : IDisposable
{
    /// <summary>
    /// 修改定时器设置
    /// </summary>
    void Change(int period);

    /// <summary>
    /// 停止定时器并释放资源
    /// </summary>
    void Stop();
}

/// <summary>
/// 定时器类型
/// </summary>
public enum TimerType : byte
{
    /// <summary>
    /// 多媒体定时器
    /// </summary>
    Multimedia,

    /// <summary>
    /// 防重入定时器
    /// </summary>
    AntiReentry
}
