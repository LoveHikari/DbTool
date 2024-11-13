using System.IO;
using System.Windows;
using System.Windows.Input;
using DbTool.Bean;
using DbTool.Models;
using Hikari.Common.Cryptography;
using Hikari.Common.IO;
using Hikari.Mvvm.Command;

namespace DbTool.ViewModels;

public class PageSettingsViewModel
{
    public PageSettingsModel Model { get; set; }

    public PageSettingsViewModel()
    {
        this.Model = new PageSettingsModel();
        // 读取配置文件
        this.Model.DbSet = App.DbSet;
        this.Model.DefaultDbTypeList = ["Sql Server", "MySql", "SQLite"];
        this.Model.CultureList = ["zh-CN", "en-US"];
    }

    public ICommand UpdateCommand
    {
        get
        {
            return new DelegateCommand((o) =>
            {
                string configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DbTool.config");
                var dbSet = System.Text.Json.JsonSerializer.Serialize(this.Model.DbSet);
                var crypto = new AESCrypto("awerfdgg");
                var b = crypto.EncryptBase64(dbSet);
                FileHelper.Write(configurationPath, b);
                HandyControl.Controls.MessageBox.Success("修改成功", "提示");
            });
        }

    }

}