using System;
using System.Collections.Generic;
using System.Text;
using Win.Models;

namespace Win.Common.Builder.MVC
{
    /// <summary>
    /// 生成数据层
    /// </summary>
    public class BuilderDAL
    {
        #region 字段
        private List<ColumnModel> _fieldList;  //选择的字段集合
        private string _modeLpath; //model命名空间
        private string _dalPath;  //数据层命名空间
        #endregion

        private string _iDalPath;  //数据层接口命名空间

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dalPath">数据层命名空间</param>
        /// <param name="modeLpath">model命名空间</param>
        /// <param name="fieldList">选择的字段集合</param>
        public BuilderDAL(string dalPath, string modeLpath, List<ColumnModel> fieldList)
        {
            _dalPath = dalPath;
            _iDalPath = "I" + dalPath;
            _fieldList = fieldList;
            _modeLpath = modeLpath;
        }
        /// <summary>
        /// 创建代码
        /// </summary>
        /// <returns></returns>
        public string CreatIDAL()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine($"using {_iDalPath};");
            strclass.AppendLine($"using {_modeLpath};");
            strclass.AppendLine("namespace " + _dalPath);
            strclass.AppendLine("{");

            strclass.AppendSpaceLine(4, "/// <summary>");
            if (_fieldList[0].TableDescription.Length > 0)
            {
                strclass.AppendSpaceLine(4, "/// " + _fieldList[0].TableDescription + "数据层");
            }
            else
            {
                strclass.AppendSpaceLine(4, "/// " + _fieldList[0].TableName + " 数据层");
            }
            strclass.AppendSpaceLine(4, "/// </summary>");
            strclass.AppendSpaceLine(4, $"public class {_fieldList[0].TableName.ToPascal()}Repository:BaseRepository<{_fieldList[0].TableName.ToPascal()}>, I{_fieldList[0].TableName.ToPascal()}Repository");
            strclass.AppendSpaceLine(4, "{");
            strclass.AppendSpaceLine(8, "");
            strclass.AppendSpaceLine(4, "}");

            strclass.AppendLine("}");
            return strclass.ToString();
        }
    }
}