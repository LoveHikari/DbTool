using CommunityToolkit.Mvvm.Input;
using DbTool.Models;
using System.Windows.Input;
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
        this.Model.CultureList = ["zh-CN", "en-US", "ja-JP"];
    }

    public ICommand UpdateCommand
    {
        get
        {
            return new RelayCommand<object>((o) =>
            {
                ConfigManager.Instance.UpdateConfig(this.Model.DbSet);
                
                HandyControl.Controls.MessageBox.Success("修改成功", "提示");
                App.Restart();
            });
        }

    }

}