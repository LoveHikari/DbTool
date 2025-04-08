using Hikari.Mvvm;
using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using DbTool.Bean;

namespace DbTool.Models
{
    public partial class PageCodeFirstModel : ObservableObject
    {
        private string _modelFilePath; // 模型文件路径
        /// <summary>
        /// 模型文件路径
        /// </summary>
        public string ModelFilePath
        {
            get => _modelFilePath;
            set { _modelFilePath = value; OnPropertyChanged(); }
        }
        
        // 是否生成数据库描述
        private bool _isGenerateDbDescription = true;
        /// <summary>
        /// 是否生成数据库描述
        /// </summary>
        public bool IsGenerateDbDescription
        {
            get => _isGenerateDbDescription;
            set { _isGenerateDbDescription = value; OnPropertyChanged(); }
        }
        // 表结构
        [ObservableProperty]
        private List<TableStructureTreeNode> _tableStructure;
        // sql文本
        private string _sqlText;
        /// <summary>
        /// sql文本
        /// </summary>
        public string SqlText
        {
            get => _sqlText;
            set { _sqlText = value; OnPropertyChanged(); }
        }
    }
}