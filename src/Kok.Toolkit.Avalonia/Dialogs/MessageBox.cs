using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Dto;
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
    public static async void ShowAsync(string message, string title = "提示")
        => await GetMessageBox(message, title, ButtonEnum.Ok).ShowAsync();

    /// <summary>
    /// 显示一条询问信息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    /// <returns></returns>
    public static async Task<bool> AskAsync(string message, string title = "询问")
    {
        var result = await GetMessageBox(message, title, ButtonEnum.YesNo).ShowAsync();
        return result is ButtonResult.Yes;
    }

    private static IMsBox<ButtonResult> GetMessageBox(string message, string title, ButtonEnum button)
    {
        return MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams()
        {
            ButtonDefinitions = button,
            CanResize = false,
            ContentHeader = title,
            ContentMessage = message,
            ContentTitle = string.Empty,
            ShowInCenter = true,
            Topmost = true,
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            SystemDecorations = SystemDecorations.BorderOnly,
            EnterDefaultButton = ClickEnum.Yes,
            EscDefaultButton = ClickEnum.No
        });
    }
}
