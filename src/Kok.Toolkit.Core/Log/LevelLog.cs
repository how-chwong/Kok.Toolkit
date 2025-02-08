namespace Kok.Toolkit.Core.Log;

/// <summary>
/// 不同等级的日志输出到不同的文件
/// </summary>
public class LevelLog : Logger, IDisposable
{
    private readonly Dictionary<LogLevel, Logger> _logs = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    public LevelLog()
    {
        MinLogLevel = LogLevel.Info;
        var config = ReadConfig();
        if (config != null)
        {
            Enable = config.LevelLogConfig.Enable;
            MinLogLevel = config.TextFileLogConfig.MinLogLevel;
        }

        if (!Enable) return;

        foreach (var level in Enum.GetValues<LogLevel>())
        {
            if (level >= MinLogLevel)
                _logs[level] = new FileLog(level, config?.LevelLogConfig);
        }
    }

    /// <summary>
    /// 设置日志子目录
    /// </summary>
    /// <param name="path"></param>
    public void SetSubSystemPath(string path)
    {
        if (_logs.Count == 0) return;
        foreach (var log in _logs.Values)
        {
            if (log is FileLog fileLog) fileLog.SetSubSystemPath(path);
        }
    }

    ///<inheritdoc />
    protected internal override void Write(LogLevel level, string message)
    {
        if (_logs.TryGetValue(level, out var log)) log.Write(level, message);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        if (_logs.Count == 0) return;
        foreach (var log in _logs.Values)
        {
            if (log is IDisposable temp) temp.Dispose();
        }
    }
}
