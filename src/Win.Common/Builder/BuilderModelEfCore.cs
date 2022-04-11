using Hikari.Common;
using System.Collections.Generic;
using System.Text;
using Win.Models;

namespace Win.Common.Builder
{
    /// <summary>
    /// 可系列化的Model代码生成组件
    /// </summary>
    public class BuilderModelEfCore : BuilderModel
    {
        public BuilderModelEfCore(List<ColumnModel> fieldList, string modelPath, string modelPrefix, string modelSuffix) : base(fieldList, modelPath, modelPrefix, modelSuffix)
        {
        }
        /// <summary>
        /// 生成完整Model类
        /// </summary>
        public override string CreatModel()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine("using System;");
            strclass.AppendLine("using Microsoft.EntityFrameworkCore;");
            strclass.AppendLine("using System.ComponentModel.DataAnnotations;");
            strclass.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            strclass.AppendLine("");
            strclass.AppendLine("namespace " + _modelpath + ";");
            strclass.AppendLine("");
            strclass.AppendSpaceLine(0, "/// <summary>");
            if (_fieldlist[0].TableDescription.Length > 0)
            {
                strclass.AppendSpaceLine(0, "/// " + _fieldlist[0].TableDescription.Replace("\r\n", "\r\n\t///"));
            }
            else
            {
                strclass.AppendSpaceLine(0, "/// " + _fieldlist[0].TableName + ":实体类(属性说明自动提取数据库字段的描述信息)");
            }
            strclass.AppendSpaceLine(0, "/// </summary>");
            strclass.AppendSpaceLine(0, $"[Serializable, Table(\"{_fieldlist[0].TableName}\")]");
            strclass.AppendSpaceLine(0, "public sealed class " + _modelName);
            strclass.AppendSpaceLine(0, "{");
            strclass.AppendLine(CreatModelMethod());
            strclass.AppendSpaceLine(0, "}");
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
            strclass.AppendSpaceLine(4, "#region Model");
            foreach (ColumnModel field in _fieldlist)
            {
                string columnType = Win.Common.CodeCommon.DbTypeToCS(field.TypeName);
                string isnull = "";
                if (field.IsCanNull)
                {
                    isnull = "?";//代表可空类型
                }

                strclass2.AppendSpaceLine(4, "/// <summary>");
                strclass2.AppendSpaceLine(4, "/// " + field.Description);
                strclass2.AppendSpaceLine(4, "/// </summary>");
                if (field.IsPrimaryKey)
                {
                    strclass2.AppendSpaceLine(4, "[Key]");
                }

                if (new List<string>() { "varchar" }.Contains(field.TypeName.ToLower()))
                {
                    strclass2.AppendSpaceLine(4, $"[Column(\"{field.ColumnName}\", TypeName = \"{field.TypeName}({field.Length})\")]");
                    strclass2.AppendSpaceLine(4, $"[StringLength({field.Length})]");
                }
                else if(new List<string>() { "decimal" }.Contains(field.TypeName.ToLower()))
                {
                    strclass2.AppendSpaceLine(4, $"[Column(\"{field.ColumnName}\", TypeName = \"{field.TypeName}({field.Length}, {field.Scale})\")]");
                }
                else
                {
                    strclass2.AppendSpaceLine(4, $"[Column(\"{field.ColumnName}\")]");
                }


                if (field.IsIdentity)
                {
                    strclass2.AppendSpaceLine(4, "[DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
                }
                if (!field.IsCanNull)  // 不为空
                {
                    strclass2.AppendSpaceLine(4, "[Required(AllowEmptyStrings = true)]");
                }

                if (!string.IsNullOrWhiteSpace(field.Description))
                {
                    strclass2.AppendSpaceLine(4, $"[Comment(\"{field.Description}\")]");
                }
                
                strclass2.AppendSpaceLine(4, "public " + columnType + isnull + " " + field.ColumnName.ToPascalCase() + " { get; set; }");//属性
            }
            strclass.Append(strclass2);
            strclass.AppendSpaceLine(4, "#endregion Model");

            return strclass.ToString();
        }




    }
}
