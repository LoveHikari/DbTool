using Hikari.Common;
using System.Collections.Generic;
using System.Text;
using Win.Models;

namespace Win.Common.Builder
{
    /// <summary>
    /// ��ϵ�л���Model�����������
    /// </summary>
    public class BuilderModel
    {
        #region �ֶ�
        private string _modelpath;  //ʵ����������ռ�
        private List<ColumnModel> _fieldlist;  //ѡ����ֶμ���
        private string _modelName;  //ʵ������
        #endregion

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="fieldList">ѡ����ֶμ���</param>
        /// <param name="modelPath">ʵ����������ռ�</param>
        /// <param name="modelPrefix">ʵ�����ǰ׺</param>
        /// <param name="modelSuffix">ʵ����ĺ�׺</param>
        public BuilderModel(List<ColumnModel> fieldList, string modelPath, string modelPrefix, string modelSuffix)
        {
            
            _fieldlist = fieldList;
            _modelpath = modelPath;
            string tableName = _fieldlist[0].TableName;
            _modelName = modelPrefix + tableName.ToPascalCase() + modelSuffix;
        }


        /// <summary>
        /// ��������Model��
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
                strclass.AppendSpaceLine(1, "/// " + _fieldlist[0].TableName + ":ʵ����(����˵���Զ���ȡ���ݿ��ֶε�������Ϣ)");
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
        /// ����ʵ���������
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
                    isnull = "?";//����ɿ�����
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
                if (!field.IsCanNull)  // ��Ϊ��
                {
                    strclass2.AppendSpaceLine(2, "[Required(AllowEmptyStrings = true)]");
                }

                if (!string.IsNullOrWhiteSpace(field.Description))
                {
                    strclass2.AppendSpaceLine(2, $"[Comment(\"{field.Description}\")]");
                }
                
                strclass2.AppendSpaceLine(2, "public " + columnType + isnull + " " + field.ColumnName.ToPascalCase() + " { get; set; }");//����
            }
            strclass.Append(strclass2);
            strclass.AppendSpaceLine(2, "#endregion Model");

            return strclass.ToString();
        }




    }
}
