using System.Text.Json;

namespace Kok.Toolkit.Log;

/// <summary>
/// 日志
/// </summary>
public abstract class Logger : ILog
{
    /// <summary>
    /// 使能
    /// </summary>
    public bool Enable { get; set; } = true;

    /// <summary>
    /// 最小日志等级
    /// </summary>
    public LogLevel MinLogLevel { get; set; } = LogLevel.Debug;

    /// <inheritdoc />
    public void Debug(string message) => Log(LogLevel.Debug, message);

    /// <inheritdoc />
    public void Info(string message) => Log(LogLevel.Info, message);

    /// <inheritdoc />
    public void Warn(string message) => Log(LogLevel.Warn, message);

    /// <inheritdoc />
    public void Error(string message) => Log(LogLevel.Error, message);

    /// <inheritdoc />
    public void Fatal(string message) => Log(LogLevel.Fatal, message);

    /// <inheritdoc />
    public void ChangeMiniLevel(LogLevel level) => MinLogLevel = level;

    /// <summary>
    /// 输出行为
    /// </summary>
    /// <param name="level"></param>
    /// <param name="message"></param>
    protected internal abstract void Write(LogLevel level, string message);

    private void Log(LogLevel level, string message)
    {
        if (Enable && level >= MinLogLevel) Write(level, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}    [{level}] {message}");
    }

    /// <summary>
    /// 配置文件名称
    /// </summary>
    private const string s_configFileName = "logger.json";

    /// <summary>
    /// 读取配置
    /// </summary>
    /// <returns></returns>
    protected static LogConfig? ReadConfig()
    {
        var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, s_configFileName);
        if (!File.Exists(file)) return null;
        var str = File.ReadAllText(file);
        return JsonSerializer.Deserialize<LogConfig>(str);
    }
}
