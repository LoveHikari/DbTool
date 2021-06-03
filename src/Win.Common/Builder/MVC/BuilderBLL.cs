using System;
using System.Collections.Generic;
using System.Text;
using Win.Models;

namespace Win.Common.Builder.MVC
{
    public class BuilderBLL
    {
        #region 字段
        private List<ColumnModel> _fieldList;  //选择的字段集合
        private string _modeLpath; //model命名空间
        private string _bllPath;  //业务层命名空间
        private string _dalPath; //数据层命名空间
        #endregion

        private string _iBllPath;  //业务层接口命名空间

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bllPath">业务层命名空间</param>
        /// <param name="modeLpath">model命名空间</param>
        /// <param name="dalPath">数据层命名空间</param>
        /// <param name="fieldList">选择的字段集合</param>
        public BuilderBLL(string bllPath, string modeLpath, string dalPath, List<ColumnModel> fieldList)
        {
            _bllPath = bllPath;
            _iBllPath = "I" + bllPath;
            _fieldList = fieldList;
            _modeLpath = modeLpath;
            _dalPath = dalPath;
        }
        /// <summary>
        /// 创建代码
        /// </summary>
        /// <returns></returns>
        public string CreatBLL()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine($"using {_dalPath};");
            strclass.AppendLine($"using {_iBllPath};");
            strclass.AppendLine($"using {_modeLpath};");
            strclass.AppendLine("namespace " + _bllPath);
            strclass.AppendLine("{");

            strclass.AppendSpaceLine(4, "/// <summary>");
            if (_fieldList[0].TableDescription.Length > 0)
            {
                strclass.AppendSpaceLine(4, "/// " + _fieldList[0].TableDescription + "业务层");
            }
            else
            {
                strclass.AppendSpaceLine(4, "/// " + _fieldList[0].TableName + " 业务层");
            }
            strclass.AppendSpaceLine(4, "/// </summary>");
            strclass.AppendSpaceLine(4, $"public class {_fieldList[0].TableName.ToPascal()}Service:BaseService<{_fieldList[0].TableName.ToPascal()}>, I{_fieldList[0].TableName.ToPascal()}Service");
            strclass.AppendSpaceLine(4, "{");
            strclass.AppendSpaceLine(8, $"public {_fieldList[0].TableName.ToPascal()}Service() : base(RepositoryFactory.{_fieldList[0].TableName.ToPascal()}Repository)");
            strclass.AppendSpaceLine(8, "{");
            strclass.AppendSpaceLine(8, "}");
            strclass.AppendSpaceLine(4, "}");

            strclass.AppendLine("}");
            return strclass.ToString();
        }
    }
}