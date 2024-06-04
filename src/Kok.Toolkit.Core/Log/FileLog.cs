using System.Collections.Concurrent;

namespace Kok.Toolkit.Core.Log;

/// <summary>
/// 文本文件日志
/// </summary>
public class FileLog : Logger, IDisposable
{
    #region 属性

    /// <summary>
    /// 单个日志文件的最大文件大小，单位kb
    /// </summary>
    public long MaxFileSizeInKb { get; private set; } = 102400;

    /// <summary>
    /// 目录下允许同时存在的最大文件个数
    /// </summary>
    public int MaxFileCount { get; private set; } = 10;

    /// <summary>
    /// 日志文件所在目录
    /// </summary>
    public string RootPath { get; private set; } = "Logs";

    /// <summary>
    /// 日志的二级目录
    /// </summary>
    public string SubSystemPath { get; private set; } = string.Empty;

    /// <summary>
    /// 日志文件目录
    /// </summary>
    public string LogDirectory => Path.Combine(RootPath, SubSystemPath);

    /// <summary>
    /// 文件名格式
    /// {0}：时间值，可自行指定时间格式；
    /// {1}：日志级别，包含该格式表示日志按级别分文件存储；
    /// {2}:文件索引,包含该格式表示限制当天日志文件的大小，超过允许的最大值则生成新文件存储
    /// </summary>
    public string FileNameFormat { get; private set; } = "{0:yyyyMMdd}.{1}.{2}.log";

    /// <summary>
    /// 是否单文件存储
    /// </summary>
    public bool IsSingleFile => !(FileNameFormat.Contains("{1}") && FileNameFormat.Contains("{2}"));

    /// <summary>
    /// 是否按日志级别存储
    /// </summary>
    public bool IsLevelFile => FileNameFormat.Contains("{1}");

    /// <summary>
    /// 是否限制单文件大小
    /// </summary>
    public bool IsLimitFile => FileNameFormat.Contains("{2}");

    #endregion 属性

    /// <summary>
    /// 构造文本日志实例
    /// </summary>
    public FileLog()
    {
        MinLogLevel = LogLevel.Info;
        var config = ReadConfig();
        InitConfig(config?.TextFileLogConfig);
        if (!Directory.Exists(RootPath)) Directory.CreateDirectory(RootPath);
        _timer = new Timer(DoLogWork, null, 0, 5000);
    }

    /// <summary>
    /// 构造文本日志实例
    /// </summary>
    /// <param name="level">日志等级</param>
    /// <param name="config">文件配置</param>
    public FileLog(LogLevel level, TextFileLogConfig? config)
    {
        MinLogLevel = level;
        Enable = true;
        InitConfig(config);
        if (!Directory.Exists(RootPath)) Directory.CreateDirectory(RootPath);
        _timer = new Timer(DoLogWork, null, 0, 5000);
    }

    private void InitConfig(TextFileLogConfig? config)
    {
        if (config == null) return;

        Enable = config.Enable;
        MinLogLevel = config.MinLogLevel;
        MaxFileSizeInKb = config.MaxFileSizeInKb < 1024 ? 1024 : config.MaxFileSizeInKb;
        MaxFileCount = config.MaxFileCount < 10 ? 10 : config.MaxFileCount;
        FileNameFormat = string.IsNullOrWhiteSpace(config.FileNameFormat) ? FileNameFormat : config.FileNameFormat;
        RootPath = string.IsNullOrWhiteSpace(config.RootPath) ? RootPath : config.RootPath;
    }

    private readonly Timer _timer;

    private readonly ConcurrentQueue<LogRecord> _logQueue = new();

    ///<inheritdoc/>
    protected internal override void Write(LogLevel level, string message) =>
        _logQueue.Enqueue(new LogRecord(DateTime.Now, level, message));

    /// <summary>
    /// 设置子系统日志目录
    /// </summary>
    /// <param name="path"></param>
    public void SetSubSystemPath(string path)
    {
        SubSystemPath = path;
        if (!Directory.Exists(LogDirectory))
            Directory.CreateDirectory(LogDirectory);
    }

    private int _writing;

    private string _currentFile = string.Empty;

    /// <summary>
    /// 处理日志
    /// </summary>
    private void DoLogWork(object? state)
    {
        if (Interlocked.Exchange(ref _writing, 1) == 0)
        {
            try
            {
                WriteFile();
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                Interlocked.Exchange(ref _writing, 0);
            }
        }

        DeleteLogFile();
    }

    private StreamWriter? _writer;

    /// <summary>
    /// 构建文件流
    /// </summary>
    /// <param name="logFile"></param>
    /// <returns></returns>
    private StreamWriter? BuildWriter(string logFile)
    {
        try
        {
            var stream = new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            var writer = new StreamWriter(stream, Encoding.UTF8);
            return writer;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 将日志内容写入文件
    /// </summary>
    private void WriteFile()
    {
        if (_logQueue.IsEmpty) return;
        var logFile = GetLatestFile();
        if (string.IsNullOrEmpty(logFile)) return;
        if (!_currentFile.Equals(logFile))
        {
            _writer?.Dispose();
            _writer = null;
            _currentFile = logFile;
        }
        _writer ??= BuildWriter(logFile);
        if (_writer == null) return;

        while (_logQueue.TryDequeue(out var log))
        {
            _writer.Write($"{log.Content}");
            _writer.WriteLine();
        }

        _writer?.Flush();
        _writer?.Dispose();
        _writer = null;
    }

    /// <summary>
    /// 获取最新的日志文件
    /// </summary>
    /// <returns></returns>
    private string GetLatestFile()
    {
        var time = DateTime.Now;
        if (!IsLimitFile || MaxFileSizeInKb == 0)
            return Path.Combine(LogDirectory, string.Format(FileNameFormat, time, MinLogLevel));

        for (var i = 1; i < int.MaxValue; i++)
        {
            var logFile = Path.Combine(LogDirectory, string.Format(FileNameFormat, time, MinLogLevel, i));
            var fileInfo = new FileInfo(logFile);
            if (!fileInfo.Exists || fileInfo.Length < MaxFileSizeInKb * 1024)
                return logFile;
        }
        return string.Empty;
    }

    /// <summary>
    /// 删除超限文件
    /// </summary>
    private void DeleteLogFile()
    {
        if (!Directory.Exists(LogDirectory))
            return;

        var files = new DirectoryInfo(LogDirectory)
            .GetFiles($"*{MinLogLevel}*{Path.GetExtension(FileNameFormat)}")
            .OrderBy(f => f.CreationTime).ToList();
        if (files.Count <= MaxFileCount)
            return;

        var num = files.Count - MaxFileCount;
        for (var i = 0; i < num; i++)
        {
            try
            {
                Console.WriteLine($"{files[i].FullName}被删除");
                File.Delete(files[i].FullName);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        _timer.Change(-1, int.MaxValue);
        _timer.Dispose();
        DoLogWork(null);
        _writer?.Dispose();
        _writer = null;
    }
}
