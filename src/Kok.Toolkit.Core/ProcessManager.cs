using System.Diagnostics;

namespace Kok.Toolkit.Core;

/// <summary>
/// 进程管理器
/// </summary>
public static class ProcessManager
{
    /// <summary>
    /// 创建一个进程
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    private static Process CreateProcess(string fileName, string? arguments = null)
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        var path = Path.GetDirectoryName(fileName);
        if (!string.IsNullOrWhiteSpace(path)) process.StartInfo.WorkingDirectory = path;
        if (arguments != null) process.StartInfo.Arguments = arguments;
        return process;
    }

    /// <summary>
    /// 启动一个进程
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="arguments"></param>
    /// <param name="onExited"></param>
    /// <returns></returns>
    public static (bool result, string error, Process? process) Start(string fileName, string arguments, Action<object, EventArgs>? onExited = null)
    {
        if (!File.Exists(fileName)) return (false, $"指定的文件不存在 {fileName}", null);
        var p = CreateProcess(fileName, arguments);
        if (onExited != null)
        {
            p.EnableRaisingEvents = true;
            p.Exited += new EventHandler(onExited!);
        }
        p.Start();
        return (true, string.Empty, p);
    }

    /// <summary>
    /// 停止指定进程
    /// </summary>
    /// <param name="process"></param>
    public static void Stop(Process process)
    {
        process?.Kill();
        process?.Dispose();
    }

    /// <summary>
    /// 停止指定进程
    /// </summary>
    /// <param name="pid"></param>
    public static void Stop(int pid)
    {
        using var process = Process.GetProcessById(pid);
        process.Kill();
    }
}
