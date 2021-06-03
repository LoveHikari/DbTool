using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Win.Models;

namespace Win.Common.Builder
{
    public class BuilderDALForSqlite : BuilderDAL
    {
        public BuilderDALForSqlite(List<ColumnModel> fieldlist, string dbherlpername, string modelpath, string dalpath, string modelSuffix, string dalSuffix) : base(fieldlist, dbherlpername, modelpath, dalpath, modelSuffix, dalSuffix)
        {

        }
        /// <summary>
        /// 得到整个类的代码
        /// </summary>
        public override string CreatDal(bool maxid, bool exists, bool add, bool update, bool delete, bool getModel, bool list)
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine("using System;");
            strclass.AppendLine("using System.Data;");
            strclass.AppendLine("using System.Text;");
            strclass.AppendLine("using System.Data.SqlClient;");
            strclass.AppendLine("using DotNet.Utilities.DBHelper;");
            strclass.AppendLine("using System.Collections.Generic;");

            strclass.AppendLine("using " + Modelpath + ";//Please add references");
            strclass.AppendLine("namespace " + Dalpath);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// 数据访问类:" + Fieldlist[0].TableName + ": " + Fieldlist[0].TableDescription);
            strclass.AppendSpaceLine(1, "/// </summary>");
            strclass.AppendSpace(1, "public partial class " + Dalname);

            strclass.AppendLine("");
            strclass.AppendSpaceLine(1, "{");

            #region 单例模式代码

            strclass.AppendLine(CreatInstance());

            #endregion

            strclass.AppendSpaceLine(2, "#region  自动生成");

            #region  方法代码

            //if (Maxid)
            //{
            //    strclass.AppendLine(CreatGetMaxID());
            //}
            //if (Exists)
            //{
            //    strclass.AppendLine(CreatExists());
            //}
            if (add)
            {
                strclass.AppendLine(CreatAdd());
            }
            if (update)
            {
                strclass.AppendLine(CreatUpdate());
            }
            if (delete)
            {
                strclass.AppendLine(CreatDelete());
            }
            if (getModel)
            {
                strclass.AppendLine(CreatGetModel());
                //strclass.AppendLine(CreatDataRowToModel());
            }
            if (list)
            {
                strclass.AppendLine(CreatGetList());
                strclass.AppendLine(CreatGetListByPage());
                strclass.AppendLine(CreatGetListByPageProc());
            }

            #endregion

            strclass.AppendSpaceLine(2, "#endregion");

            strclass.AppendSpaceLine(1, "}");
            strclass.AppendLine("}");
            strclass.AppendLine("");

            return strclass.ToString();
        }
        /// <summary>
        /// 得到Add()的代码
        /// </summary>
        public override string CreatAdd()
        {
            StringBuilder strclass = new StringBuilder();
            StringBuilder strclass2 = new StringBuilder();  //parameters参数

            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 增加一条数据");
            strclass.AppendSpaceLine(2, "/// </summary>");
            string strretu = "bool";  // 主键类型
            if (IsHasIdentity)
            {
                strretu = "int";
                if (IdentityKeyType != "int")
                {
                    strretu = IdentityKeyType;
                }
            }

            //方法定义头
            string strFun = StringHelper.Space(2) + "public " + strretu + " Add(" + Modelname + " model)";
            strclass.AppendLine(strFun);
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "CrDB db = new DBHelper();");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpaceLine(3, "strSql.Append(\"insert into " + Fieldlist[0].TableName + "(\");");
            string[] columnList = (from f in Fieldlist where !f.IsIdentity select f.ColumnName).ToArray();
            strclass.AppendSpaceLine(3, $"strSql.Append(\"{string.Join(",", columnList)})\");");
            strclass.AppendSpaceLine(3, "strSql.Append(\" values (\");");
            strclass.AppendSpaceLine(3, $"strSql.Append(\"@{string.Join(",", columnList).Replace(",", ",@")})\");");
            strclass.AppendSpaceLine(3, "strSql.Append(\";select last_insert_rowid();\");");

            strclass2.AppendSpaceLine(3, "List<DBParam> dbParams = new List<DBParam>(new DBParam[] ");
            strclass2.AppendSpaceLine(4, "{");
            foreach (ColumnModel field in Fieldlist)
            {
                if (field.IsIdentity)
                {
                    continue;  //如果是自增列，则跳过
                }

                strclass2.AppendSpaceLine(5, $"new DBParam(\"{PreParameter + field.ColumnName}\",model.{field.ColumnName.ToFirstUpper()}, DbType.{Win.Common.CodeCommon.SqlTypeToDbType(field.TypeName)},{field.Precision}),");
            }
            strclass2.DelLastChar(",");
            strclass2.AppendSpaceLine(4, "});");
            strclass.AppendLine(strclass2.ToString());
            //重新定义方法头
            if (strretu == "void")
            {
                strclass.AppendSpaceLine(3, "" + DbhelperName + ".ExecuteScalar(strSql.ToString(),dbParams);");
            }
            else if (strretu == "bool")
            {
                strclass.AppendSpaceLine(3,
                    "int rows=" + DbhelperName + ".ExecuteNonQuery(strSql.ToString(),dbParams);");
                strclass.AppendSpaceLine(3, "if (rows > 0)");
                strclass.AppendSpaceLine(3, "{");
                strclass.AppendSpaceLine(4, "return true;");
                strclass.AppendSpaceLine(3, "}");
                strclass.AppendSpaceLine(3, "else");
                strclass.AppendSpaceLine(3, "{");
                strclass.AppendSpaceLine(4, "return false;");
                strclass.AppendSpaceLine(3, "}");
            }
            else
            {
                strclass.AppendSpaceLine(3,
                    "object obj = " + DbhelperName + ".ExecuteScalar(strSql.ToString(),dbParams);");
                strclass.AppendSpaceLine(3, "if (obj == null)");
                strclass.AppendSpaceLine(3, "{");
                strclass.AppendSpaceLine(4, "return 0;");
                strclass.AppendSpaceLine(3, "}");
                strclass.AppendSpaceLine(3, "else");
                strclass.AppendSpaceLine(3, "{");
                switch (strretu)
                {
                    case "int":
                        strclass.AppendSpaceLine(4, "return Convert.ToInt32(obj);");
                        break;
                    case "long":
                        strclass.AppendSpaceLine(4, "return Convert.ToInt64(obj);");
                        break;
                    case "decimal":
                        strclass.AppendSpaceLine(4, "return Convert.ToDecimal(obj);");
                        break;
                }

                strclass.AppendSpaceLine(3, "}");
            }
            strclass.AppendSpace(2, "}");
            return strclass.ToString();
        }
    }
}