using MsBox.Avalonia;
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
    public static async void ShowAsync(string message, string title = "信息")
        => await MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.Ok).ShowAsync();

    /// <summary>
    /// 显示一条询问信息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public static async Task<bool> AskAsync(string message, string title = "询问")
    {
        var result = await MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.YesNo).ShowAsync();
        return result is ButtonResult.Yes;
    }
}
