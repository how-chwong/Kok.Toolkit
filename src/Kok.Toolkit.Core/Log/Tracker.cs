using System.Collections.Concurrent;

namespace Kok.Toolkit.Core.Log;

/// <summary>
/// 日志跟踪器
/// </summary>
public static class Tracker
{
    private static readonly ConcurrentDictionary<Type, Logger> s_loggers = new();

    /// <summary>
    /// 静态构造
    /// </summary>
    static Tracker()
    {
        AddLogger(new LevelLog());
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
    }

    private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
    {
        if (s_loggers.IsEmpty) return;

        foreach (var item in s_loggers.Values)
        {
            if (item is IDisposable log) log.Dispose();
        }
    }

    private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        if (e.Observed) return;
        foreach (var ex in e.Exception.Flatten().InnerExceptions)
        {
            WriteError(ex.ToString());
        }
        e.SetObserved();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex) WriteError(ex.ToString());
        if (e.IsTerminating) WriteFatal(e.ToString()!);
    }

    #region 启用日志处理器

    /// <summary>
    /// 增加日志处理器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="logger"></param>
    public static void AddLogger<T>(T logger) where T : Logger
    {
        if (s_loggers.TryGetValue(typeof(T), out var oldLogger))
        {
            s_loggers.TryUpdate(typeof(T), logger, oldLogger);
            if (oldLogger is IDisposable temp) temp.Dispose();
        }
        else
        {
            s_loggers.TryAdd(typeof(T), logger);
        }
    }

    /// <summary>
    /// 移除日志处理器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="logger"></param>
    public static void RemoveLogger<T>(T logger) where T : Logger
    {
        if (s_loggers.ContainsKey(typeof(T))) s_loggers.Remove(typeof(T), out _);
    }

    /// <summary>
    /// 清空所有日志处理器
    /// </summary>
    public static void ClearLogger()
    {
        if (s_loggers.IsEmpty) return;
        foreach (var logger in s_loggers.Values)
        {
            if (logger is IDisposable temp) temp.Dispose();
        }
        s_loggers.Clear();
    }

    /// <summary>
    /// 修改指定类型的日志处理器的最小日志级别
    /// </summary>
    /// <param name="type">日志处理器类型</param>
    /// <param name="level">最小日志级别</param>
    /// <returns></returns>
    public static bool ChangeMinLogLevel(Type type, LogLevel level)
    {
        if (!s_loggers.TryGetValue(type, out var logger)) return false;
        logger.ChangeMiniLevel(level);
        return true;
    }

    #endregion 启用日志处理器

    #region 日志输出

    /// <summary>
    /// 输出一条日志信息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="level"></param>
    private static void WriteLine(string message, LogLevel level)
    {
        if (s_loggers.IsEmpty) return;
        foreach (var log in s_loggers.Values)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    log.Debug(message);
                    break;

                case LogLevel.Info:
                    log.Info(message);
                    break;

                case LogLevel.Warn:
                    log.Warn(message);
                    break;

                case LogLevel.Error:
                    log.Error(message);
                    break;

                case LogLevel.Fatal:
                    log.Fatal(message);
                    break;

                case LogLevel.All:
                default:
                    log.Info(message);
                    break;
            }
        }
    }

    /// <summary>
    /// 输出调试信息
    /// </summary>
    /// <param name="message">日志内容</param>
    public static void WriteDebug(string message) => WriteLine(message, LogLevel.Debug);

    /// <summary>
    /// 输出信息
    /// </summary>
    /// <param name="message">日志内容</param>
    public static void WriteInfo(string message) => WriteLine(message, LogLevel.Info);

    /// <summary>
    /// 输出警告信息
    /// </summary>
    /// <param name="message">日志内容</param>
    public static void WriteWarn(string message) => WriteLine(message, LogLevel.Warn);

    /// <summary>
    /// 输出错误信息
    /// </summary>
    /// <param name="message">日志内容</param>
    public static void WriteError(string message) => WriteLine(message, LogLevel.Error);

    /// <summary>
    /// 输出严重故障信息
    /// </summary>
    /// <param name="message">日志内容</param>
    public static void WriteFatal(string message) => WriteLine(message, LogLevel.Fatal);

    #endregion 日志输出
}
