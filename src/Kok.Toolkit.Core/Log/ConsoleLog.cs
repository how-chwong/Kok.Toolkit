namespace Kok.Toolkit.Core.Log;

/// <summary>
/// 控制台日志
/// </summary>
public class ConsoleLog : Logger
{
    /// <summary>
    /// 构造一个控制台输出日志
    /// </summary>
    public ConsoleLog()
    {
        var cfg = ReadConfig();
        if (cfg == null) return;
        MinLogLevel = cfg.ConsoleLogConfig.MinLogLevel;
        Enable = cfg.ConsoleLogConfig.Enable;
    }

    ///<inheritdoc />
    protected internal override void Write(LogLevel level, string message)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = level switch
        {
            LogLevel.Debug => ConsoleColor.Green,
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Warn => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Fatal => ConsoleColor.DarkRed,
            _ => ConsoleColor.Blue
        };
        Console.WriteLine(message);
        Console.ForegroundColor = old;
    }
}
