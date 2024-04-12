namespace Kok.Toolkit.Log;

/// <summary>
/// 日志配置
/// </summary>
public class LogConfig
{
    /// <summary>
    /// 控制台日志配置
    /// </summary>
    public ConsoleLogConfig ConsoleLogConfig { get; set; } = new();

    /// <summary>
    /// 文本文件日志配置
    /// </summary>
    public TextFileLogConfig TextFileLogConfig { get; set; } = new();

    /// <summary>
    /// 等级日志配置
    /// </summary>
    public TextFileLogConfig LevelLogConfig { get; set; } = new();
}

/// <summary>
/// 控制台日志配置
/// </summary>
public class ConsoleLogConfig
{
    /// <summary>
    /// 最小日志级别
    /// </summary>
    public LogLevel MinLogLevel { get; set; } = LogLevel.Debug;

    /// <summary>
    /// 使能
    /// </summary>
    public bool Enable { get; set; } = true;
}

/// <summary>
/// 文本文件日志配置
/// </summary>
public class TextFileLogConfig : ConsoleLogConfig
{
    /// <summary>
    /// 单文件允许的最大文件大小，单位KB
    /// 最小值为1024
    /// </summary>
    public long MaxFileSizeInKb { get; set; } = 102400;

    /// <summary>
    /// 根目录下允许的最大文件个数
    /// </summary>
    public int MaxFileCount { get; set; } = 10;

    /// <summary>
    /// 日志文件名格式，默认
    /// </summary>
    public string FileNameFormat { get; set; } = "{0:yyyyMMdd}.{1}.{2}.log";

    /// <summary>
    /// 日志文件根目录，支持相对路径和绝对路径
    /// </summary>
    public string RootPath { get; set; } = "Logs";

    /// <summary>
    /// 构造函数
    /// </summary>
    public TextFileLogConfig()
    {
        MinLogLevel = LogLevel.Info;
    }
}
