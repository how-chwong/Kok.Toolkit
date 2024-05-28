namespace Kok.Toolkit.Core.Log;

/// <summary>
/// 日志记录
/// </summary>
/// <param name="Time">日志时间</param>
/// <param name="Level">日志级别</param>
/// <param name="Content">日志内容</param>
public record LogRecord(DateTime Time, LogLevel Level, string Content);
