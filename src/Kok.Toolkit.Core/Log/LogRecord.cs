namespace Kok.Toolkit.Core.Log;

/// <summary>
/// 日志记录
/// </summary>
public class LogRecord
{
    /// <summary>
    /// 日志时间
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// 日志级别
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// 日志内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 构造一条日志
    /// </summary>
    /// <param name="time"></param>
    /// <param name="level"></param>
    /// <param name="message"></param>
    public LogRecord(DateTime time, LogLevel level, string message)
    {
        Time = time;
        Level = level;
        Content = message;
    }
}
