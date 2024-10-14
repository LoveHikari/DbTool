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
        public BuilderModelJavaJpa(List<ColumnModel> fieldList, string modelPath, string modelPrefix,
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
            strclass.AppendLine("package " + _modelPath + ";");
            strclass.AppendLine("");
            strclass.AppendLine("import lombok.Data;");
            strclass.AppendLine("import lombok.experimental.Accessors;");
            strclass.AppendLine("import org.springframework.context.annotation.Description;");
            strclass.AppendLine("import javax.persistence.*;");
            strclass.AppendLine("import java.io.Serializable;");
            strclass.AppendLine("import java.util.Date;");
            strclass.AppendLine("");
            strclass.AppendSpaceLine(0, "/**");
            if (_fieldList[0].TableDescription.Length > 0)
            {
                strclass.AppendSpaceLine(0, " * " + _fieldList[0].TableDescription.Replace("\r\n", "\r\n\t///"));
            }
            else
            {
                strclass.AppendSpaceLine(0, " * " + _fieldList[0].TableName + ":实体类(属性说明自动提取数据库字段的描述信息)");
            }
            strclass.AppendSpaceLine(0, " */");
            strclass.AppendSpaceLine(0, "@Data");
            strclass.AppendSpaceLine(0, "@Accessors(chain = true)");
            strclass.AppendSpaceLine(0, "@Entity");
            strclass.AppendSpaceLine(0, $"@Table(name = \"{_fieldList[0].TableName}\")");
            strclass.AppendSpaceLine(0, $"@org.hibernate.annotations.Table(appliesTo = \"{_fieldList[0].TableName}\", comment = \"{_fieldList[0].TableDescription}\")");
            strclass.AppendSpaceLine(0, $"public class {_modelName} implements Serializable " + "{");
            strclass.AppendLine(CreatModelMethod());
            strclass.AppendSpaceLine(0, "}");
            strclass.AppendLine("");

            return strclass.ToString();
        }

        public override string CreatRepositoryInterface()
        {
            throw new System.NotImplementedException();
        }

        public override string CreatRepository()
        {
            throw new System.NotImplementedException();
        }

        public override string CreatApplicationInterface()
        {
            throw new System.NotImplementedException();
        }

        public override string CreatApplication()
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// 生成实体类的属性
        /// </summary>
        /// <returns></returns>
        private string CreatModelMethod()
        {
            StringBuilder strclass = new StringBuilder();
            StringBuilder strclass2 = new StringBuilder();

            foreach (ColumnModel field in _fieldList)
            {
                string columnType = Win.Common.CodeCommon.DbTypeToJava(field.TypeName);
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
                else
                {
                    s += ", nullable = true";
                }
                if (!string.IsNullOrWhiteSpace(field.Description))
                {
                    s += $", columnDefinition=\"{field.Description}\"";
                }
                strclass2.AppendSpaceLine(4, $"@Column({s})");

                strclass2.AppendSpaceLine(4, $"private {columnType} {field.ColumnName.ToCamelCase()};  // {field.Description}");
            }
            strclass.Append(strclass2);
            //strclass.AppendSpaceLine(4, "#endregion Model");

            return strclass.ToString();
        }

    }
}
