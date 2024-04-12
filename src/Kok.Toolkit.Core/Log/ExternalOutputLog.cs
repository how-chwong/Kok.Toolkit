namespace Kok.Toolkit.Log;

/// <summary>
/// 外部输出日志
/// </summary>
public class ExternalOutputLog : Logger
{
    private readonly Func<string, Task> _externalActionAsync;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="actionAsync"></param>
    public ExternalOutputLog(Func<string, Task> actionAsync)
        => _externalActionAsync = actionAsync;

    /// <inheritdoc/>
    protected internal override async void Write(LogLevel level, string message)
    {
        try
        {
            await _externalActionAsync(message);
        }
        catch (Exception e)
        {
            throw new Exception($"日志外部输出处理错误：{e.Message}");
        }
    }
}
