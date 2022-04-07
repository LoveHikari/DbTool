using Hikari.Common;
using System.Collections.Generic;
using System.Text;
using Win.Models;

namespace Win.Common.Builder
{
    /// <summary>
    /// 可系列化的Model代码生成组件
    /// </summary>
    public class BuilderModel
    {
        #region 字段
        private string _modelpath;  //实体类的命名空间
        private List<ColumnModel> _fieldlist;  //选择的字段集合
        private string _modelName;  //实体类名
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
        public string CreatModel()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine("using System;");
            strclass.AppendLine("using Microsoft.EntityFrameworkCore;");
            strclass.AppendLine("using System.ComponentModel.DataAnnotations;");
            strclass.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            strclass.AppendLine("namespace " + _modelpath + ";");
            strclass.AppendSpaceLine(1, "/// <summary>");
            if (_fieldlist[0].TableDescription.Length > 0)
            {
                strclass.AppendSpaceLine(1, "/// " + _fieldlist[0].TableDescription.Replace("\r\n", "\r\n\t///"));
            }
            else
            {
                strclass.AppendSpaceLine(1, "/// " + _fieldlist[0].TableName + ":实体类(属性说明自动提取数据库字段的描述信息)");
            }
            strclass.AppendSpaceLine(1, "/// </summary>");
            strclass.AppendSpaceLine(1, $"[Serializable, Table(\"{_fieldlist[0].TableName}\")]");
            strclass.AppendSpaceLine(1, "public sealed class " + _modelName);
            strclass.AppendSpaceLine(1, "{");
            strclass.AppendLine(CreatModelMethod());
            strclass.AppendSpaceLine(1, "}");
            strclass.AppendLine("");

            return strclass.ToString();
        }


        /// <summary>
        /// 生成实体类的属性
        /// </summary>
        /// <returns></returns>
        private string CreatModelMethod()
        {
            StringBuilder strclass = new StringBuilder();
            StringBuilder strclass2 = new StringBuilder();
            strclass.AppendSpaceLine(2, "#region Model");
            foreach (ColumnModel field in _fieldlist)
            {
                string columnType = Win.Common.CodeCommon.DbTypeToCS(field.TypeName);
                string isnull = "";
                if (field.IsCanNull)
                {
                    isnull = "?";//代表可空类型
                }

                strclass2.AppendSpaceLine(2, "/// <summary>");
                strclass2.AppendSpaceLine(2, "/// " + field.Description);
                strclass2.AppendSpaceLine(2, "/// </summary>");
                if (field.IsPrimaryKey)
                {
                    strclass2.AppendSpaceLine(2, "[Key]");
                }

                if (new List<string>() { "varchar" }.Contains(field.TypeName.ToLower()))
                {
                    strclass2.AppendSpaceLine(2, $"[Column(\"{field.ColumnName}\", TypeName = \"{field.TypeName}({field.Length})\")]");
                    strclass2.AppendSpaceLine(2, $"[StringLength({field.Length})]");
                }
                else if(new List<string>() { "decimal" }.Contains(field.TypeName.ToLower()))
                {
                    strclass2.AppendSpaceLine(2, $"[Column(\"{field.ColumnName}\", TypeName = \"{field.TypeName}({field.Length}, {field.Scale})\")]");
                }
                else
                {
                    strclass2.AppendSpaceLine(2, $"[Column(\"{field.ColumnName}\")]");
                }


                if (field.IsIdentity)
                {
                    strclass2.AppendSpaceLine(2, "[DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
                }
                if (!field.IsCanNull)  // 不为空
                {
                    strclass2.AppendSpaceLine(2, "[Required(AllowEmptyStrings = true)]");
                }

                if (!string.IsNullOrWhiteSpace(field.Description))
                {
                    strclass2.AppendSpaceLine(2, $"[Comment(\"{field.Description}\")]");
                }
                
                strclass2.AppendSpaceLine(2, "public " + columnType + isnull + " " + field.ColumnName.ToPascalCase() + " { get; set; }");//属性
            }
            strclass.Append(strclass2);
            strclass.AppendSpaceLine(2, "#endregion Model");

            return strclass.ToString();
        }




    }
}
