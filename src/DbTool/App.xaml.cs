using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DbTool.Bean;
using DbTool.Properties.Langs;
using HandyControl.Tools;
using Hikari.Common.Cryptography;
using Hikari.Common.IO;
using Win.Common.Config;

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
            ConfigHelper.Instance.SetLang(lang);
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
            HandyControl.Controls.MessageBox.Show(e.Exception.Message, "UI线程全局异常", MessageBoxButton.OK, MessageBoxImage.Error);
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
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                HandyControl.Controls.MessageBox.Show(exception.Message, "非UI线程全局异常", MessageBoxButton.OK, MessageBoxImage.Error);
                //LogHelper.WriteError(exception, "非UI线程全局异常");
                //throw exception;
            }
        }

    }
}
