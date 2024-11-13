using Hikari.Mvvm;

namespace DbTool.Bean;

public class DbSet: NotificationObject
{
    private string _defaultConnString; // 默认数据库连接字符串

    public string DefaultConnString
    {
        get => _defaultConnString;
        set { _defaultConnString = value; NotifyPropertyChanged(); }
    }
    private string _defaultDbType; // 默认数据库类型

    public string DefaultDbType
    {
        get => _defaultDbType;
        set { _defaultDbType = value; NotifyPropertyChanged(); }
    }
    private string _defaultCulture; // 默认本地化

    public string DefaultCulture
    {
        get => _defaultCulture;
        set { _defaultCulture = value; NotifyPropertyChanged(); }
    }
    private bool _globalUsingEnabled; // 全局引用

    public bool GlobalUsingEnabled
    {
        get => _globalUsingEnabled;
        set { _globalUsingEnabled = value; NotifyPropertyChanged(); }
    }
    private bool _nullableReferenceTypesEnabled; // 可空引用类型

    public bool NullableReferenceTypesEnabled
    {
        get => _nullableReferenceTypesEnabled;
        set { _nullableReferenceTypesEnabled = value; NotifyPropertyChanged(); }
    }
    private bool _fileScopedNamespaceEnabled; // 文件范围命名空间

    public bool FileScopedNamespaceEnabled
    {
        get => _fileScopedNamespaceEnabled;
        set { _fileScopedNamespaceEnabled = value; NotifyPropertyChanged(); }
    }
}