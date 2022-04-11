using Hikari.Common;
using System.Collections.Generic;
using System.Text;
using Win.Models;

namespace Win.Common.Builder
{
    /// <summary>
    /// 可系列化的Model代码生成组件
    /// </summary>
    public abstract class BuilderModel
    {
        #region 字段
        protected string _modelpath;  //实体类的命名空间
        protected List<ColumnModel> _fieldlist;  //选择的字段集合
        protected string _modelName;  //实体类名
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fieldList">选择的字段集合</param>
        /// <param name="modelPath">实体类的命名空间</param>
        /// <param name="modelPrefix">实体类的前缀</param>
        /// <param name="modelSuffix">实体类的后缀</param>
        public BuilderModel(List<ColumnModel> fieldList, string modelPath, string modelPrefix, string modelSuffix)
        {
            
            _fieldlist = fieldList;
            _modelpath = modelPath;
            string tableName = _fieldlist[0].TableName;
            _modelName = modelPrefix + tableName.ToPascalCase() + modelSuffix;
        }


        /// <summary>
        /// 生成完整Model类
        /// </summary>
        public abstract string CreatModel();





    }
}
