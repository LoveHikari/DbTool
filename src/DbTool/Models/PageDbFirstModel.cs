using System.Collections.Generic;
using System.Data;
using Hikari.WPF.MVVM;

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

        public string ModelPath
        {
            get => _modelPath;
            set { _modelPath = value; NotifyPropertyChanged(); }
        }
        private string _modelPrefix;  // 前缀

        public string ModelPrefix
        {
            get => _modelPrefix;
            set { _modelPrefix = value; NotifyPropertyChanged(); }
        }
        private string _modelSuffix;  // 后缀

        public string ModelSuffix
        {
            get => _modelSuffix;
            set { _modelSuffix = value; NotifyPropertyChanged(); }
        }
    }
}