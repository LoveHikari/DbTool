using System;
using System.Collections.Generic;
using System.Text;
using Win.Models;

namespace Win.Common.Builder.MVC
{
    /// <summary>
    /// 生成业务层接口
    /// </summary>
    public class BuilderIBLL
    {
        #region 字段
        private List<ColumnModel> _fieldList;  //选择的字段集合
        private string _modeLpath; //model命名空间
        #endregion

        private string _iBllPath;  //业务层接口命名空间

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bllPath">业务层命名空间</param>
        /// <param name="modeLpath">model命名空间</param>
        /// <param name="fieldList">选择的字段集合</param>
        public BuilderIBLL(string bllPath, string modeLpath, List<ColumnModel> fieldList)
        {
            _iBllPath = "I" + bllPath;
            _fieldList = fieldList;
            _modeLpath = modeLpath;
        }
        /// <summary>
        /// 创建代码
        /// </summary>
        /// <returns></returns>
        public string CreatIBLL()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine($"using {_modeLpath};");
            strclass.AppendLine("namespace " + _iBllPath);
            strclass.AppendLine("{");

            strclass.AppendSpaceLine(4, "/// <summary>");
            if (_fieldList[0].TableDescription.Length > 0)
            {
                strclass.AppendSpaceLine(4, "/// " + _fieldList[0].TableDescription + "业务接口");
            }
            else
            {
                strclass.AppendSpaceLine(4, "/// " + _fieldList[0].TableName + " 业务接口");
            }
            strclass.AppendSpaceLine(4, "/// </summary>");
            strclass.AppendSpaceLine(4, $"public interface I{_fieldList[0].TableName.ToPascal()}Service:IBaseService<{_fieldList[0].TableName.ToPascal()}>");
            strclass.AppendSpaceLine(4, "{");
            strclass.AppendSpaceLine(8, "");
            strclass.AppendSpaceLine(4, "}");

            strclass.AppendLine("}");
            return strclass.ToString();
        }
    }
}