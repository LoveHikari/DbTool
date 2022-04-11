using Hikari.Common;
using System.Collections.Generic;
using System.Text;
using Win.Models;

namespace Win.Common.Builder
{
    /// <summary>
    /// 可系列化的Model代码生成组件
    /// </summary>
    public class BuilderModelJavaJpa : BuilderModel
    {
        public BuilderModelJavaJpa(List<ColumnModel> fieldList, string modelPath, string modelPrefix, string modelSuffix) : base(fieldList, modelPath, modelPrefix, modelSuffix)
        {
        }
        /// <summary>
        /// 生成完整Model类
        /// </summary>
        public override string CreatModel()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine("package " + _modelpath + ";");
            strclass.AppendLine("");
            strclass.AppendLine("import lombok.Data;");
            strclass.AppendLine("import lombok.experimental.Accessors;");
            strclass.AppendLine("import org.springframework.context.annotation.Description;");
            strclass.AppendLine("import javax.persistence.*;");
            strclass.AppendLine("import java.io.Serializable;");
            strclass.AppendLine("");
            strclass.AppendSpaceLine(0, "/**");
            if (_fieldlist[0].TableDescription.Length > 0)
            {
                strclass.AppendSpaceLine(0, " * " + _fieldlist[0].TableDescription.Replace("\r\n", "\r\n\t///"));
            }
            else
            {
                strclass.AppendSpaceLine(0, " * " + _fieldlist[0].TableName + ":实体类(属性说明自动提取数据库字段的描述信息)");
            }
            strclass.AppendSpaceLine(0, " */");
            strclass.AppendSpaceLine(0, "@Data");
            strclass.AppendSpaceLine(0, "@Accessors(chain = true)");
            strclass.AppendSpaceLine(0, "@Entity");
            strclass.AppendSpaceLine(0, $"@Table(name = \"{_fieldlist[0].TableName}\")");
            strclass.AppendSpaceLine(0, $"@org.hibernate.annotations.Table(appliesTo = \"{_fieldlist[0].TableName}\", comment = \"{_fieldlist[0].TableDescription}\")");
            strclass.AppendSpaceLine(0, $"public class {_modelName} implements Serializable " + "{");
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

            foreach (ColumnModel field in _fieldlist)
            {
                string columnType = Win.Common.CodeCommon.DbTypeToCS(field.TypeName);
                string isnull = "";
                if (field.IsCanNull)
                {
                    isnull = "?";//代表可空类型
                }

                strclass2.AppendSpaceLine(4, "/**");
                strclass2.AppendSpaceLine(4, " * " + field.Description);
                strclass2.AppendSpaceLine(4, " */");
                if (field.IsPrimaryKey)
                {
                    strclass2.AppendSpaceLine(4, "@Id");
                }
                if (field.IsIdentity)
                {
                    strclass2.AppendSpaceLine(4, "@GeneratedValue(strategy = GenerationType.IDENTITY)");
                }

                string s = $"name = \"{field.ColumnName}\"";
                if (!field.IsCanNull)  // 不为空
                {
                    s += ", nullable = false";
                }
                if (!string.IsNullOrWhiteSpace(field.Description))
                {
                    s += $", columnDefinition=\"{field.Description}\"";
                }
                strclass2.AppendSpaceLine(4, $"@Column({s})");

                strclass2.AppendSpaceLine(4, $"private {columnType + isnull} {field.ColumnName.ToPascalCase()};  // {field.Description}");
            }
            strclass.Append(strclass2);
            //strclass.AppendSpaceLine(4, "#endregion Model");

            return strclass.ToString();
        }

    }
}
