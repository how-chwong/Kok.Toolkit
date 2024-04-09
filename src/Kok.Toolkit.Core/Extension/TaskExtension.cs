namespace Kok.Toolkit.Core.Extension;

/// <summary>
/// Task扩展
/// </summary>
public static class TaskExtension
{
    /// <summary>
    /// 一直异步等待，直到指定的表达式为True
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="waitMilliseconds"></param>
    /// <returns></returns>
    public static Task WaitAsync(Func<bool>? expression, int waitMilliseconds = 1000)
    {
        if (expression == null || expression.Invoke()) return Task.CompletedTask;
        return Task.Run(() =>
        {
            var temp = waitMilliseconds;
            while (expression.Invoke() == false || temp > 0)
            {
                Thread.Sleep(100);
                temp -= 100;
            }
        });
    }
}