using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace DbTool.ViewModels
{
    public class MainWindowViewModel
    {
        /// <summary>
        /// 关闭窗口
        /// </summary>
        public ICommand ClosingCommand => new RelayCommand<object>((obj) =>
        {
            CancelEventArgs? e = obj as CancelEventArgs;
            if (HandyControl.Controls.MessageBox.Ask("确定要关闭软件？") == MessageBoxResult.Cancel)
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