using System.IO;
using System.Windows;
using System.Windows.Input;
using DbTool.Bean;
using DbTool.Models;
using Hikari.Common.Cryptography;
using Hikari.Common.IO;
using Hikari.Mvvm.Command;
using Win.Common.Config;

namespace DbTool.ViewModels;

public class PageSettingsViewModel
{
    public PageSettingsModel Model { get; set; }

    public PageSettingsViewModel()
    {
        this.Model = new PageSettingsModel();
        // 读取配置文件
        this.Model.DbSet = ConfigManager.Instance.ConfigParm;
        this.Model.DefaultDbTypeList = ["Sql Server", "MySql", "SQLite"];
        this.Model.CultureList = ["zh-CN", "en-US"];
    }

    public ICommand UpdateCommand
    {
        get
        {
            return new DelegateCommand((o) =>
            {
                ConfigManager.Instance.UpdateConfig(this.Model.DbSet);
                HandyControl.Controls.MessageBox.Success("修改成功", "提示");
            });
        }

    }

}