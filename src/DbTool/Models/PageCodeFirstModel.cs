using Hikari.Mvvm;
using System.Data;
using DbTool.Bean;

namespace DbTool.Models
{
    public class PageCodeFirstModel : NotificationObject
    {
        private string _modelFilePath; // 模型文件路径
        /// <summary>
        /// 模型文件路径
        /// </summary>
        public string ModelFilePath
        {
            get => _modelFilePath;
            set { _modelFilePath = value; NotifyPropertyChanged(); }
        }
        
        // 是否生成数据库描述
        private bool _isGenerateDbDescription = true;
        /// <summary>
        /// 是否生成数据库描述
        /// </summary>
        public bool IsGenerateDbDescription
        {
            get => _isGenerateDbDescription;
            set { _isGenerateDbDescription = value; NotifyPropertyChanged(); }
        }
        // 表结构
        private List<TableStructureTreeNode> _tableStructure;

        /// <summary>
        /// 表结构
        /// </summary>
        public List<TableStructureTreeNode> TableStructure
        {
            get => _tableStructure;
            set { _tableStructure = value; NotifyPropertyChanged(); }
        }
        // sql文本
        private string _sqlText;
        /// <summary>
        /// sql文本
        /// </summary>
        public string SqlText
        {
            get => _sqlText;
            set { _sqlText = value; NotifyPropertyChanged(); }
        }
    }
}