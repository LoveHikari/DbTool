using Hikari.Mvvm;
using System.Data;

namespace DbTool.Models
{
    public class PageDbFirstModel : NotificationObject
    {
        private string _connectionString; // 数据库连接字符串

        public string ConnectionString
        {
            get => _connectionString;
            set { _connectionString = value; NotifyPropertyChanged(); }
        }
        private List<string> _providerNameList;

        public List<string> ProviderNameList  // 数据库类型
        {
            get => _providerNameList;
            set { _providerNameList = value; NotifyPropertyChanged(); }
        }
        private string _providerName;  // 数据库类型

        public string ProviderName
        {
            get => _providerName;
            set { _providerName = value; NotifyPropertyChanged(); }
        }

        private DataView _tableList;   // 表列表

        public DataView TableList
        {
            get => _tableList;
            set { _tableList = value; NotifyPropertyChanged(); }
        }

        private DataView _tableFieldList;  // 表字段列表
        public DataView TableFieldList
        {
            get => _tableFieldList;
            set { _tableFieldList = value; NotifyPropertyChanged(); }
        }
        private List<string> _primaryKeyList;  // 主键列表
        public List<string> PrimaryKeyList
        {
            get => _primaryKeyList;
            set { _primaryKeyList = value; NotifyPropertyChanged(); }
        }

        private string _codeContent;  // 代码内容

        public string CodeContent
        {
            get => _codeContent;
            set { _codeContent = value; NotifyPropertyChanged(); }
        }
        private int _codeLanguage;  // 编程语言

        public int CodeLanguage
        {
            get => _codeLanguage;
            set { _codeLanguage = value; NotifyPropertyChanged(); }
        }
        private string _modelPath;  // 命名空间
        /// <summary>
        /// model命名空间
        /// </summary>
        public string ModelPath
        {
            get => _modelPath;
            set { _modelPath = value; NotifyPropertyChanged(); }
        }
        private string _modelPrefix;  // 前缀
        /// <summary>
        /// model前缀
        /// </summary>
        public string ModelPrefix
        {
            get => _modelPrefix;
            set { _modelPrefix = value; NotifyPropertyChanged(); }
        }
        private string _modelSuffix;  // 后缀
        /// <summary>
        /// model后缀
        /// </summary>
        public string ModelSuffix
        {
            get => _modelSuffix;
            set { _modelSuffix = value; NotifyPropertyChanged(); }
        }
        // Repository命名空间
        private string _repositoryPath;
        /// <summary>
        /// Repository命名空间
        /// </summary>
        public string RepositoryPath
        {
            get => _repositoryPath;
            set { _repositoryPath = value; NotifyPropertyChanged(); }
        }
        // Repository前缀
        private string _repositoryPrefix;

        /// <summary>
        /// Repository前缀
        /// </summary>
        public string RepositoryPrefix
        {
            get => _repositoryPrefix;
            set { _repositoryPrefix = value;NotifyPropertyChanged(); }
        }
        private string _repositorySuffix;

        /// <summary>
        /// Repository后缀
        /// </summary>
        public string RepositorySuffix
        {
            get => _repositorySuffix;
            set { _repositorySuffix = value;NotifyPropertyChanged(); }
        }
        // Application命名空间
        private string _applicationPath;

        public string ApplicationPath
        {
            get => _applicationPath;
            set { _applicationPath = value; NotifyPropertyChanged(); }
        }
        private string _applicationPrefix;

        /// <summary>
        /// Application前缀
        /// </summary>
        public string ApplicationPrefix
        {
            get => _applicationPrefix;
            set { _applicationPrefix = value; NotifyPropertyChanged(); }
        }
        private string _applicationSuffix;

        /// <summary>
        /// Application后缀
        /// </summary>
        public string ApplicationSuffix
        {
            get => _applicationSuffix;
            set { _applicationSuffix = value; NotifyPropertyChanged(); }
        }

        private string _currentConfig;
        /// <summary>
        /// 当前选择的配置
        /// </summary>
        public string CurrentConfig
        {
            get => _currentConfig;
            set { _currentConfig = value; NotifyPropertyChanged(); }
        }
    }
}