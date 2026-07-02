using Avalonia.Controls;
using Avalonia.VisualTree;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace GKG.UI.General
{
    /// <summary>
    /// 消息弹窗
    /// </summary>
    public class MessageBox
    {
        /// <summary>
        /// 显示确认对话框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">内容</param>
        /// <param name="parentViewReference">父视图对象</param>
        /// <returns></returns>
        public static async Task<ButtonResult> ShowConfirmDialog(string title, string message, Control? parentViewReference)
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                title,
                message,
                ButtonEnum.YesNo,
                Icon.Question
            );

            var parentWindow = parentViewReference?.GetVisualRoot() as Window;
            return parentWindow != null
                ? await messageBox.ShowAsPopupAsync(parentWindow)
                : await messageBox.ShowAsync();
        }

        /// <summary>
        /// 显示错误对话框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="message">内容</param>
        /// <param name="parentViewReference">父视图对象</param>
        /// <returns></returns>
        public static async Task ShowErrorDialog(string title, string message, Control? parentViewReference)
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                title,
                message,
                ButtonEnum.Ok,
                Icon.Error
            );

            var parentWindow = parentViewReference?.GetVisualRoot() as Window;
            if (parentWindow != null)
                await messageBox.ShowAsPopupAsync(parentWindow);
            else
                await messageBox.ShowAsync();
        }

        /// <summary>
        /// 显示消息对话框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="parentViewReference">父视图对象</param>
        /// <returns></returns>
        public static async Task ShowInfoDialog(string title, string message, Control? parentViewReference)
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.Ok, Icon.Info);
            var parentWindow = parentViewReference?.GetVisualRoot() as Window;
            if (parentWindow != null)
                await messageBox.ShowAsPopupAsync(parentWindow);
            else
                await messageBox.ShowAsync();
        }
    }
}
