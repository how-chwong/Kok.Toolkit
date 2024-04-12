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
}
