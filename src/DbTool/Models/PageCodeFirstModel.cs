using Hikari.Mvvm;
using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using DbTool.Bean;
using System.Collections.Generic;

namespace DbTool.Models
{
    public partial class PageCodeFirstModel : ObservableObject
    {
        private string _modelFilePath; // 模型文件路径
        /// <summary>
        /// 模型文件路径（保持向后兼容）
        /// </summary>
        public string ModelFilePath
        {
            get => _modelFilePath;
            set { _modelFilePath = value; OnPropertyChanged(); }
        }
        
        private List<string> _modelFilePaths = new List<string>(); // 模型文件路径集合
        /// <summary>
        /// 模型文件路径集合
        /// </summary>
        public List<string> ModelFilePaths
        {
            get => _modelFilePaths;
            set { _modelFilePaths = value; OnPropertyChanged(); }
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