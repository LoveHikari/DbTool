using DbTool.Properties.Langs;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using Win.Common.Config;
using Wpf.Ui.Controls;

namespace DbTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            Current.DispatcherUnhandledException += App_OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var lang = ConfigManager.Instance.ConfigParm.DefaultCulture ?? "zh-cn";
            LangProvider.Instance.Culture = new CultureInfo(lang);
            //ConfigHelper.Instance.SetLang(lang);
        }
        private static Mutex _mutex = null;


        protected override void OnStartup(StartupEventArgs e)
        {
            string appName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "DbTool";
            _mutex = new Mutex(true, appName, out bool createdNew);
            if (!createdNew)
            {
                // 程序已运行，退出当前实例
                Application.Current.Shutdown();
                return;
            }
            base.OnStartup(e);
        }
        /// <summary>
        /// 重启程序
        /// </summary>
        public static void Restart()
        {
            _mutex.ReleaseMutex(); // 释放锁
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            location = location.Replace(".dll", ".exe");
            Process.Start(location); // 启动新实例
            Application.Current.Shutdown(); // 关闭当前实例
        }
        /// <summary>
        /// UI线程抛出全局异常事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LoadingService.HideLoading();
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "UI线程全局异常",
                Content = e.Exception.Message,
                //CloseButtonIcon = new SymbolIcon(SymbolRegular.Dismiss24),

            };
            uiMessageBox.ShowDialogAsync();
            //System.Windows.MessageBox.Show(e.Exception.Message, "UI线程全局异常", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            //LogHelper.WriteError(e.Exception, "UI线程全局异常");
            e.Handled = true;
            //throw e.Exception;
        }

        /// <summary>
        /// 非UI线程抛出全局异常事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                LoadingService.HideLoading();
                var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "非UI线程全局异常",
                    Content = exception.Message,
                    //CloseButtonIcon = new SymbolIcon(SymbolRegular.Dismiss24),

                };
                uiMessageBox.ShowDialogAsync();
               // System.Windows.MessageBox.Show(exception.Message, "非UI线程全局异常",  System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                //LogHelper.WriteError(exception, "非UI线程全局异常");
                //throw exception;
            }
        }

    }
}
