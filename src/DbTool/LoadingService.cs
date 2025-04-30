using DbTool.Views.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace DbTool;

public class LoadingService
{
    private static LoadingDialog? _loadingDialog;
    private static Window? _currentWindow;
    private static Panel? _hostPanel;

    public static void ShowLoading(string? message = null)
    {
        if (Application.Current.Dispatcher.CheckAccess())
        {
            ShowLoadingInternal(message);
        }
        else
        {
            Application.Current.Dispatcher.Invoke(() => ShowLoadingInternal(message));
        }
    }

    private static void ShowLoadingInternal(string? message)
    {
        _currentWindow = Application.Current.MainWindow;
        if (_currentWindow == null) return;
        HideLoadingInternal();

        _loadingDialog = new LoadingDialog()
        {
            Message = message ?? "加载中...",
        };

        if (_currentWindow.Content is Panel mainPanel)
        {
            _hostPanel = mainPanel;
            _hostPanel.Children.Add(_loadingDialog);
        }
        else
        {
            var grid = new Grid();
            grid.Children.Add((UIElement)_currentWindow.Content);
            grid.Children.Add(_loadingDialog);
            _currentWindow.Content = grid;
            _hostPanel = grid;
        }
    }

    public static void HideLoading()
    {
        if (Application.Current.Dispatcher.CheckAccess())
        {
            HideLoadingInternal();
        }
        else
        {
            Application.Current.Dispatcher.Invoke(HideLoadingInternal);
        }
    }

    private static void HideLoadingInternal()
    {
        if (_loadingDialog == null || _hostPanel == null) return;

        _hostPanel.Children.Remove(_loadingDialog);
        _loadingDialog = null;
        _hostPanel = null;
    }
}