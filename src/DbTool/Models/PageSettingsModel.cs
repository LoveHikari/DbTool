using DbTool.Bean;
using Hikari.Mvvm;
using Win.Common.Config.Model;

namespace DbTool.Models;

public class PageSettingsModel : NotificationObject
{
    private ConfigParm _dbSet;

    public ConfigParm DbSet
    {
        get => _dbSet;
        set { _dbSet = value; NotifyPropertyChanged(); }
    }
    private List<string> _defaultDbTypeList;
    public List<string> DefaultDbTypeList
    {
        get => _defaultDbTypeList;
        set { _defaultDbTypeList = value; NotifyPropertyChanged(); }
    }
    private List<string> _cultureList;
    public List<string> CultureList
    {
        get => _cultureList;
        set { _cultureList = value; NotifyPropertyChanged(); }
    }
}