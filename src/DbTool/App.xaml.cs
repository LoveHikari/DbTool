﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DbTool.Bean;
using HandyControl.Tools;
using Hikari.Common.Cryptography;
using Hikari.Common.IO;

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
            ConfigHelper.Instance.SetLang("zh-cn");
        }
        /// <summary>
        /// UI线程抛出全局异常事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "UI线程全局异常", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(exception.Message, "非UI线程全局异常", MessageBoxButton.OK, MessageBoxImage.Error);
                //LogHelper.WriteError(exception, "非UI线程全局异常");
                //throw exception;
            }
        }

    }
}
