using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

namespace DbTool.ViewModels
{
    public class MainWindowViewModel
    {
        /// <summary>
        /// 关闭窗口
        /// </summary>
        public ICommand ClosingCommand => new AsyncRelayCommand<object>(async (obj) =>
        {
            CancelEventArgs? e = obj as CancelEventArgs;
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "提示",
                Content = "确定要关闭软件？",
                PrimaryButtonText = "确认",
                CloseButtonText = "取消",
            };
            var messageBoxResult = await uiMessageBox.ShowDialogAsync();
            if (messageBoxResult == MessageBoxResult.None)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
                Environment.Exit(0);
            }
        });
    }
}