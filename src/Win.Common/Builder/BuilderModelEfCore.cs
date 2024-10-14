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
        public BuilderModelEfCore(List<ColumnModel> fieldList, string modelPath, string modelPrefix,
            string modelSuffix,
            string repositoryPath, string repositoryPrefix, string repositorySuffix,
            string applicationPath, string applicationPrefix, string applicationSuffix) : base(fieldList, modelPath,
            modelPrefix, modelSuffix, repositoryPath, repositoryPrefix, repositorySuffix, applicationPath,
            applicationPrefix, applicationSuffix)
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
            strclass.AppendLine("namespace " + _modelPath + ";");
            strclass.AppendLine("");
            strclass.AppendSpaceLine(0, "/// <summary>");
            if (_fieldList[0].TableDescription.Length > 0)
            {
                strclass.AppendSpaceLine(0, "/// " + _fieldList[0].TableDescription.Replace("\r\n", "\r\n\t///"));
            }
            else
            {
                strclass.AppendSpaceLine(0, "/// " + _fieldList[0].TableName + ":实体类(属性说明自动提取数据库字段的描述信息)");
            }
            strclass.AppendSpaceLine(0, "/// </summary>");
            strclass.AppendSpaceLine(0, $"[Serializable, Table(\"{_fieldList[0].TableName}\")]");
            strclass.AppendSpaceLine(0, "public sealed class " + _modelName);
            strclass.AppendSpaceLine(0, "{");
            strclass.AppendLine(CreatModelMethod());
            strclass.AppendSpaceLine(0, "}");
            strclass.AppendLine("");

            return strclass.ToString();
        }
        /// <summary>
        /// 生成仓储接口
        /// </summary>
        /// <returns></returns>
        public override string CreatRepositoryInterface()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine($"using {_modelPath};");
            strclass.AppendLine("");
            strclass.AppendLine($"namespace {_modelPath}.Repository.Interfaces;");
            strclass.AppendLine("");
            strclass.AppendSpaceLine(0, "/// <summary>");
            if (_fieldList[0].TableDescription.Length > 0)
            {
                strclass.AppendSpaceLine(0, "/// " + _fieldList[0].TableDescription.Replace("\r\n", "\r\n\t///") + "仓储接口");
            }
            else
            {
                strclass.AppendSpaceLine(0, "/// " + _fieldList[0].TableName + ":仓储接口");
            }
            strclass.AppendSpaceLine(0, "/// </summary>");
            strclass.AppendSpaceLine(0, $"public interface I{_repositoryName} : IBaseRepository<{_modelName}>");
            strclass.AppendSpaceLine(0, "{");
            strclass.AppendLine("");
            strclass.AppendSpaceLine(0, "}");
            strclass.AppendLine("");

            return strclass.ToString();
        }
        /// <summary>
        /// 生成仓储类
        /// </summary>
        /// <returns></returns>
        public override string CreatRepository()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine("using System;");
            strclass.AppendLine($"using {_modelPath};");
            strclass.AppendLine($"using {_modelPath}.Repository.Interfaces;");
            strclass.AppendLine("");
            strclass.AppendLine("namespace " + _repositoryPath + ";");
            strclass.AppendLine("");
            strclass.AppendSpaceLine(0, "/// <summary>");
            if (_fieldList[0].TableDescription.Length > 0)
            {
                strclass.AppendSpaceLine(0, "/// " + _fieldList[0].TableDescription.Replace("\r\n", "\r\n\t///") + "仓储类");
            }
            else
            {
                strclass.AppendSpaceLine(0, "/// " + _fieldList[0].TableName + ":仓储类");
            }
            strclass.AppendSpaceLine(0, "/// </summary>");
            strclass.AppendSpaceLine(0, $"public class {_repositoryName} : BaseRepository<{_modelName}>, I{_repositoryName}");
            strclass.AppendSpaceLine(0, "{");
            strclass.AppendSpaceLine(4, $"public {_repositoryName}(IDbContext dbContext) : base(dbContext)");
            strclass.AppendSpaceLine(4, "{");
            strclass.AppendSpaceLine(4, "}");
            strclass.AppendSpaceLine(0, "}");
            strclass.AppendLine("");

            return strclass.ToString();
        }
        /// <summary>
        /// 生成业务接口
        /// </summary>
        /// <returns></returns>
        public override string CreatApplicationInterface()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine($"using {_modelPath};");
            strclass.AppendLine("");
            strclass.AppendLine($"namespace {_applicationPath}.Interfaces;");
            strclass.AppendLine("");
            strclass.AppendSpaceLine(0, "/// <summary>");
            if (_fieldList[0].TableDescription.Length > 0)
            {
                strclass.AppendSpaceLine(0, "/// " + _fieldList[0].TableDescription.Replace("\r\n", "\r\n\t///") + "业务接口");
            }
            else
            {
                strclass.AppendSpaceLine(0, "/// " + _fieldList[0].TableName + ":业务接口");
            }
            strclass.AppendSpaceLine(0, "/// </summary>");
            strclass.AppendSpaceLine(0, $"public interface I{_applicationName} : IBaseService<{_modelName}>");
            strclass.AppendSpaceLine(0, "{");
            strclass.AppendLine("");
            strclass.AppendSpaceLine(0, "}");
            strclass.AppendLine("");

            return strclass.ToString();
        }
        /// <summary>
        /// 生成业务类
        /// </summary>
        /// <returns></returns>
        public override string CreatApplication()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine($"using {_modelPath};");
            strclass.AppendLine($"using {_applicationPath}.Interfaces;");
            strclass.AppendLine("");
            strclass.AppendLine("namespace " + _applicationPath + ";");
            strclass.AppendLine("");
            strclass.AppendSpaceLine(0, "/// <summary>");
            if (_fieldList[0].TableDescription.Length > 0)
            {
                strclass.AppendSpaceLine(0, "/// " + _fieldList[0].TableDescription.Replace("\r\n", "\r\n\t///") + "业务类");
            }
            else
            {
                strclass.AppendSpaceLine(0, "/// " + _fieldList[0].TableName + ":业务类");
            }
            strclass.AppendSpaceLine(0, "/// </summary>");
            strclass.AppendSpaceLine(0, $"public class {_applicationName} : BaseService<{_modelName}>, I{_applicationName}");
            strclass.AppendSpaceLine(0, "{");
            strclass.AppendSpaceLine(4, $"public {_applicationName}()");
            strclass.AppendSpaceLine(4, "{");
            strclass.AppendSpaceLine(4, "}");
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
            foreach (ColumnModel field in _fieldList)
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
