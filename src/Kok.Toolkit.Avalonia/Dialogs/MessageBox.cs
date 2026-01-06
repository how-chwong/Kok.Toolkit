using Kok.Toolkit.Avalonia.Hosting;
using Kok.Toolkit.Core.Log;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;

namespace Kok.Toolkit.Avalonia.Dialogs;

/// <summary>
/// 消息提示框
/// </summary>
public static class MessageBox
{
    /// <summary>
    /// 显示一条提示消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    public static async Task ShowAsync(string message, string title = "提示")
    {
        var win = AvaloniaHost.CurrentWindow ?? AvaloniaHost.MainWindow;
        if (win == null)
        {
            Tracker.WriteError("不支持在未激活的窗体弹窗");
            return;
        }
        await GetMessageBox(message, title, ButtonEnum.Ok).ShowAsPopupAsync(win);
    }

    /// <summary>
    /// 显示一条询问信息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public static async Task<bool> AskAsync(string message, string title = "询问")
    {
        var win = AvaloniaHost.CurrentWindow ?? AvaloniaHost.MainWindow;
        if (win == null)
        {
            Tracker.WriteError("不支持在未激活的窗体弹窗");
            return false;
        }

        var result = await GetMessageBox(message, title, ButtonEnum.YesNo).ShowAsPopupAsync(win);
        return result is ButtonResult.Yes;
    }

    private static IMsBox<ButtonResult> GetMessageBox(string message, string title, ButtonEnum button)
        => MessageBoxManager.GetMessageBoxStandard(title, message, button);
}
