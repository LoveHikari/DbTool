using System;
using System.Collections.Generic;
using System.Text;
using Win.Models;

namespace Win.Common.Builder.MVC
{
    public class BuilderModel : Builder.BuilderModel
    {
        public BuilderModel(List<ColumnModel> fieldlist, string modelpath, string modelSuffix) : base(fieldlist, modelpath, modelSuffix)
        {
        }

        /// <summary>
        /// 生成完整Model类
        /// </summary>
        public override string CreatModel()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine("using System;");
            strclass.AppendLine("namespace " + Modelpath);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");
            if (Fieldlist[0].TableDescription.Length > 0)
            {
                strclass.AppendSpaceLine(1, "/// " + Fieldlist[0].TableDescription.Replace("\r\n", "\r\n\t///"));
            }
            else
            {
                strclass.AppendSpaceLine(1, "/// " + Fieldlist[0].TableName + ":实体类(属性说明自动提取数据库字段的描述信息)");
            }
            strclass.AppendSpaceLine(1, "/// </summary>");
            strclass.AppendSpaceLine(1, $"[Serializable,Table(\"{Fieldlist[0].TableName}\")]");
            strclass.AppendSpaceLine(1, "public partial class " + ModelName);
            strclass.AppendSpaceLine(1, "{");
            strclass.AppendSpaceLine(2, "public " + ModelName + "()");
            strclass.AppendSpaceLine(2, "{}");
            strclass.AppendLine(CreatModelMethod());
            strclass.AppendSpaceLine(1, "}");
            strclass.AppendLine("}");
            strclass.AppendLine("");

            return strclass.ToString();
        }


        /// <summary>
        /// 生成实体类的属性
        /// </summary>
        /// <returns></returns>
        public override string CreatModelMethod()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendSpaceLine(2, "#region Model");
            foreach (ColumnModel field in Fieldlist)
            {
                string columnType = Win.Common.CodeCommon.DbTypeToCS(field.TypeName);
                string defValue = "";
                string isnull = "";
                if ((!field.IsIdentity) && (!field.IsPrimaryKey) && (field.IsCanNull) && (columnType != "string"))
                {
                    isnull = "?";//代表可空类型
                }

                strclass.AppendSpaceLine(2, "/// <summary>");
                strclass.AppendSpaceLine(2, "/// " + field.Description);
                strclass.AppendSpaceLine(2, "/// </summary>");
                if (field.IsPrimaryKey)
                {
                    strclass.AppendSpaceLine(2, "[Key]");
                }
                else
                {
                    strclass.AppendSpaceLine(2, "[Column]");

                }
                if (field.DefaultVal.Length > 0)
                {
                    switch (columnType.ToLower())
                    {
                        case "int":
                        case "long":
                            if (field.DefaultVal.Trim().IndexOf("'", StringComparison.CurrentCulture) > -1)
                            {
                                defValue = field.DefaultVal.Trim().Replace("'", "\"");
                            }
                            else
                            {
                                defValue = field.DefaultVal.Trim().Replace("(", "").Replace(")", "");
                            }
                            break;
                        case "bool":
                        case "bit":
                            {
                                string val = field.DefaultVal.Trim().Replace("'", "").ToLower();
                                if (val == "1" || val == "true")
                                {
                                    defValue = "true";
                                }
                                else
                                {
                                    defValue = "false";
                                }

                            }
                            break;
                        case "nchar":
                        case "ntext":
                        case "nvarchar":
                        case "char":
                        case "text":
                        case "varchar":
                        case "string":
                            if (field.DefaultVal.Trim().StartsWith("N'"))
                            {
                                defValue = field.DefaultVal.Trim().Remove(0, 1).Replace("'", "\"");
                            }
                            else
                            {
                                if (field.DefaultVal.Trim().IndexOf("'", StringComparison.CurrentCulture) > -1)
                                {
                                    defValue = field.DefaultVal.Trim().Replace("'", "\"");
                                }
                                else
                                {
                                    defValue = field.DefaultVal.Trim().Replace("(", "").Replace(")", "") + "\"";
                                }
                            }
                            break;
                        case "datetime":
                            if (field.DefaultVal == "getdate" ||
                                field.DefaultVal == "Now()" ||
                                field.DefaultVal == "Now" ||
                                field.DefaultVal == "CURRENT_TIME" ||
                                field.DefaultVal == "CURRENT_DATE"
                                )
                            {
                                defValue = "DateTime.Now";
                            }
                            else
                            {
                                defValue = "Convert.ToDateTime(" + field.DefaultVal.Trim().Replace("'", "\"") + ")";
                            }
                            break;
                        case "uniqueidentifier":
                            {
                                //if (field.DefaultVal == "newid")
                                //{
                                //    strclass1.Append("=" + field.DefaultVal.Trim().Replace("'", ""));
                                //}                                
                            }
                            break;
                        case "decimal":
                        case "double":
                        case "float":
                            {
                                defValue = field.DefaultVal.Replace("'", "").Replace("(", "").Replace(")", "").ToLower() + "M";
                            }
                            break;
                        //case "sys_guid()":
                        //    break;
                        default:
                            //    strclass1.Append("=" + field.DefaultVal);
                            break;

                    }
                    strclass.AppendSpaceLine(2, $"[DefaultValue(" + defValue + ")]");
                }
                if (!field.IsCanNull)
                {
                    strclass.AppendSpaceLine(2, "[Required(ErrorMessage = \"必填\",AllowEmptyStrings = true)]");
                }
                if (columnType == "string" || columnType == "Byte[]")
                {
                    strclass.AppendSpaceLine(2, "[StringLength(" + field.Length + ", MinimumLength = 0, ErrorMessage = \"{2}到{1}个字符\")]");
                }
                if (string.IsNullOrEmpty(field.Description))
                {
                    strclass.AppendSpaceLine(2, $"[Display(Name = \"{field.ColumnName}\")]");
                }
                else
                {
                    strclass.AppendSpaceLine(2, $"[Display(Name = \"{field.Description}\")]");
                }
                
                strclass.AppendSpace(2, "public " + columnType + isnull + " " + field.ColumnName.ToFirstUpper());//属性
                strclass.AppendLine("{get;set;}");
            }
            strclass.AppendSpaceLine(2, "#endregion Model");

            return strclass.ToString();
        }
    }
}