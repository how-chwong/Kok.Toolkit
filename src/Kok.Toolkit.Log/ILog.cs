namespace Kok.Toolkit.Log;

/// <summary>
/// 统一日志接口
/// </summary>
public interface ILog
{
    /// <summary>
    /// 调式输出
    /// </summary>
    /// <param name="message">日志信息</param>
    void Debug(string message);

    /// <summary>
    /// 信息输出
    /// </summary>
    /// <param name="message">日志信息</param>
    void Info(string message);

    /// <summary>
    /// 警告输出
    /// </summary>
    /// <param name="message">日志信息</param>
    void Warn(string message);

    /// <summary>
    /// 错误输出
    /// </summary>
    /// <param name="message">日志信息</param>
    void Error(string message);

    /// <summary>
    /// 致命输出
    /// </summary>
    /// <param name="message">日志信息</param>
    void Fatal(string message);

    /// <summary>
    /// 修改最小日志级别
    /// </summary>
    /// <param name="level"></param>
    void ChangeMiniLevel(LogLevel level);
}

/// <summary>
/// 日志级别
/// </summary>
public enum LogLevel : byte
{
    /// <summary>
    /// 所有日志
    /// </summary>
    All,

    /// <summary>
    /// 调试
    /// </summary>
    Debug,

    /// <summary>
    /// 信息
    /// </summary>
    Info,

    /// <summary>
    /// 警告
    /// </summary>
    Warn,

    /// <summary>
    /// 错误
    /// </summary>
    Error,

    /// <summary>
    /// 致命
    /// </summary>
    Fatal
}
