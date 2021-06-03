using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Win.Models;

namespace Win.Common.Builder
{
    /// <summary>
    /// 数据访问层代码构造器（Parameter方式）
    /// </summary>
    public class BuilderDAL
    {
        #region 字段
        private string _modelname; //model类名
        private string _dalname; //dal类名
        private List<ColumnModel> _fieldlist; //选择要生成的字段集合
        private List<ColumnModel> _keys; // 主键或条件字段的集合
        private string _dbhelperName; //数据库访问类名
        private string _dalpath; //数据层的命名空间
        private string _modelpath; //model命名空间

        private string _identityKey;  //标识列，或主键字段
        private string _identityKeyType;  //标识列，或主键字段类型
        private string _dbParaHead;  //不同数据库类的前缀
        private string _preParameter;  //存储过程参数 调用符号@
        private string _fieldstrlist;  //所选字段的 select 列表
        private bool _isHasIdentity;  //主键或条件字段中是否有标识列
        private string _keysNullTip;

        #endregion

        #region 属性
        /// <summary>
        /// 主键或条件字段中是否有标识列
        /// </summary>
        public bool IsHasIdentity => _isHasIdentity;
        /// <summary>
        /// 标识列，或主键字段类型
        /// </summary>
        public string IdentityKeyType => _identityKeyType;
        /// <summary>
        /// model类名
        /// </summary>
        public string Modelname => _modelname;
        /// <summary>
        /// 选择要生成的字段集合
        /// </summary>
        public List<ColumnModel> Fieldlist => _fieldlist;
        /// <summary>
        /// 存储过程参数 调用符号@
        /// </summary>
        public string PreParameter => _preParameter;
        /// <summary>
        /// 数据库访问类名
        /// </summary>
        public string DbhelperName=> _dbhelperName;
        /// <summary>
        /// model命名空间
        /// </summary>
        public string Modelpath => _modelpath;
        /// <summary>
        /// 数据层的命名空间
        /// </summary>
        public string Dalpath => _dalpath;
        /// <summary>
        /// dal类名
        /// </summary>
        public string Dalname => _dalname;
        #endregion


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fieldlist">字段集合</param>
        /// <param name="dbherlpername">数据库访问类名</param>
        /// <param name="modelpath"> 实体类命名空间</param>
        /// <param name="dalpath">dal命名空间</param>
        /// <param name="modelSuffix">实体类名前缀</param>
        /// <param name="dalSuffix">dal类名前缀</param>
        public BuilderDAL( List<ColumnModel> fieldlist, string dbherlpername, string modelpath, string dalpath, string modelSuffix, string dalSuffix)
        {
            _fieldlist = fieldlist;
            _dbhelperName = dbherlpername;
            _modelpath = modelpath;
            _dalpath = dalpath;
            _fieldlist = fieldlist;
            string tableName = _fieldlist[0].TableName;
            _modelname = tableName.ToPascal() + modelSuffix;
            _dalname = tableName.ToPascal() + dalSuffix;

            _keys = Win.Common.CodeCommon.GetPrimaryKeyList(fieldlist);
            foreach (ColumnModel key in _keys)
            {
                _identityKey = key.ColumnName;
                _identityKeyType = key.TypeName;
                if (key.IsIdentity || key.IsPrimaryKey)
                {
                    _identityKey = key.ColumnName;
                    _identityKeyType = CodeCommon.DbTypeToCS(key.TypeName);
                    break;
                }
            }
            StringBuilder fields = new StringBuilder();
            foreach (ColumnModel obj in _fieldlist)
            {
                fields.Append(obj.ColumnName + ",");
            }
            fields.DelLastChar(",");
            _fieldstrlist = fields.ToString();

            _isHasIdentity = Win.Common.CodeCommon.IsHasIdentity(_keys);
            _dbParaHead = "";//"Sql";
            _preParameter = "@";
            _keysNullTip = _keys.Count == 0 ? "//该表无主键信息，请自定义主键/条件字段" : "";
        }

        /// <summary>
        /// 得到整个类的代码
        /// </summary>
        public virtual string CreatDal(bool maxid, bool exists, bool add, bool update, bool delete, bool getModel, bool list)
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine("using System;");
            strclass.AppendLine("using System.Data;");
            strclass.AppendLine("using System.Text;");
            strclass.AppendLine("using System.Data.SqlClient;");
            strclass.AppendLine("using DotNet.Utilities.DBHelper;");
            strclass.AppendLine("using System.Collections.Generic;");

            strclass.AppendLine("using "+ _modelpath + ";//Please add references");
            strclass.AppendLine("namespace " + _dalpath);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");
            strclass.AppendSpaceLine(1, "/// 数据访问类:" + _fieldlist[0].TableName + ": " + _fieldlist[0].TableDescription);
            strclass.AppendSpaceLine(1, "/// </summary>");
            strclass.AppendSpace(1, "public partial class " + _dalname);

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
        /// 得到单例模式代码
        /// </summary>
        /// <returns></returns>
        public string CreatInstance()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendSpaceLine(2, "#region  instance");
            strclass.AppendSpaceLine(2, "private static volatile " + _dalname + " _instance = null;");
            strclass.AppendSpaceLine(2, "private static readonly object lockHelper = new object();");
            strclass.AppendSpaceLine(2, "public " + _dalname + "(){}");
            strclass.AppendSpaceLine(2, "public static " + _dalname + " Instance");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "get");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "if (_instance == null)");
            strclass.AppendSpaceLine(4, "{");
            strclass.AppendSpaceLine(5, "lock (lockHelper)");
            strclass.AppendSpaceLine(5, "{");
            strclass.AppendSpaceLine(6, "if (_instance == null)");
            strclass.AppendSpaceLine(7, " _instance = new " + _dalname + "();");
            strclass.AppendSpaceLine(5, "}");
            strclass.AppendSpaceLine(4, "}");
            strclass.AppendSpaceLine(4, "return _instance;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");
            strclass.AppendSpaceLine(2, "#endregion");
            return strclass.ToString();
        }

        #region 数据层(使用Parameter实现)

        ///// <summary>
        ///// 得到最大ID的方法代码
        ///// </summary>
        ///// <param name="TabName"></param>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //public string CreatGetMaxID()
        //{
        //    StringPlus strclass = new StringPlus();
        //    if (_keys.Count > 0)
        //    {
        //        string keyname = "";
        //        foreach (ColumnInfo obj in _keys)
        //        {
        //            if (CodeCommon.DbTypeToCS(obj.TypeName) == "int")
        //            {
        //                keyname = obj.ColumnName;
        //                if (obj.IsPrimaryKey)
        //                {
        //                    strclass.AppendLine("");
        //                    strclass.AppendSpaceLine(2, "/// <summary>");
        //                    strclass.AppendSpaceLine(2, "/// " + Languagelist["summaryGetMaxId"].ToString());
        //                    strclass.AppendSpaceLine(2, "/// </summary>");
        //                    strclass.AppendSpaceLine(2, "public int GetMaxId()");
        //                    strclass.AppendSpaceLine(2, "{");
        //                    strclass.AppendSpaceLine(2, "return " + DbHelperName + ".GetMaxID(\"" + keyname + "\", \"" + _tablename + "\"); ");
        //                    strclass.AppendSpaceLine(2, "}");
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return strclass.ToString();
        //}

        ///// <summary>
        ///// 得到Exists方法的代码
        ///// </summary>
        ///// <param name="_tablename"></param>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //public string CreatExists()
        //{
        //    StringPlus strclass = new StringPlus();
        //    if (_keys.Count > 0)
        //    {
        //        string strInparam = Maticsoft.CodeHelper.CodeCommon.GetInParameter(Keys, false);
        //        if (!string.IsNullOrEmpty(strInparam))
        //        {
        //            strclass.AppendSpaceLine(2, "/// <summary>");
        //            strclass.AppendSpaceLine(2, "/// " + Languagelist["summaryExists"].ToString());
        //            strclass.AppendSpaceLine(2, "/// </summary>");
        //            strclass.AppendSpaceLine(2, "public bool Exists(" + strInparam + ")");
        //            strclass.AppendSpaceLine(2, "{");
        //            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
        //            strclass.AppendSpaceLine(3, "strSql.Append(\"select count(1) from " + _tablename + "\");");
        //            strclass.AppendSpaceLine(3, "strSql.Append(\" where " + Maticsoft.CodeHelper.CodeCommon.GetWhereParameterExpression(Keys, false, dbobj.DbType) + "\");");

        //            strclass.AppendLine(CodeCommon.GetPreParameter(Keys, false, dbobj.DbType));

        //            strclass.AppendSpaceLine(3, "return " + DbHelperName + ".Exists(strSql.ToString(),parameters);");
        //            strclass.AppendSpaceLine(2, "}");
        //        }
        //    }
        //    return strclass.Value;
        //}

        /// <summary>
        /// 得到Add()的代码
        /// </summary>
        public virtual string CreatAdd()
        {
            StringBuilder strclass = new StringBuilder();
            StringBuilder strclass2 = new StringBuilder();  //parameters参数

            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 增加一条数据");
            strclass.AppendSpaceLine(2, "/// </summary>");
            string strretu = "bool";  // 主键类型
            if (_isHasIdentity)
            {
                strretu = "int";
                if (_identityKeyType != "int")
                {
                    strretu = _identityKeyType;
                }
            }

            //方法定义头
            string strFun = StringHelper.Space(2) + "public " + strretu + " Add(" + _modelname + " model)";
            strclass.AppendLine(strFun);
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "CrDB db = new DBHelper();");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpaceLine(3, "strSql.Append(\"insert into " + _fieldlist[0].TableName + "(\");");
            string[] columnList = (from f in _fieldlist where !f.IsIdentity select f.ColumnName).ToArray();
            strclass.AppendSpaceLine(3, $"strSql.Append(\"{string.Join(",", columnList)})\");");
            strclass.AppendSpaceLine(3, "strSql.Append(\" values (\");");
            strclass.AppendSpaceLine(3, $"strSql.Append(\"@{string.Join(",", columnList).Replace(",",",@")})\");");
            strclass.AppendSpaceLine(3, "strSql.Append(\";select @@IDENTITY\");");
            strclass.AppendSpaceLine(3, "//strSql.Append(\";select last_insert_rowid();\");");

            strclass2.AppendSpaceLine(3, "List<DBParam> dbParams = new List<DBParam>(new DBParam[] ");
            strclass2.AppendSpaceLine(4, "{");
            foreach (ColumnModel field in _fieldlist)
            {
                if (field.IsIdentity)
                {
                    continue;  //如果是自增列，则跳过
                }

                strclass2.AppendSpaceLine(5, $"new DBParam(\"{_preParameter + field.ColumnName}\",model.{field.ColumnName.ToFirstUpper()}, DbType.{Win.Common.CodeCommon.SqlTypeToDbType(field.TypeName)},{field.Length}),");
            }
            strclass2.DelLastChar(",");
            strclass2.AppendSpaceLine(4, "});");
            strclass.AppendLine(strclass2.ToString());
            //重新定义方法头
            if (strretu == "void")
            {
                strclass.AppendSpaceLine(3, "" + _dbhelperName + ".ExecuteScalar(strSql.ToString(),dbParams);");
            }
            else if (strretu == "bool")
            {
                strclass.AppendSpaceLine(3,
                    "int rows=" + _dbhelperName + ".ExecuteNonQuery(strSql.ToString(),dbParams);");
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
                    "object obj = " + _dbhelperName + ".ExecuteScalar(strSql.ToString(),dbParams);");
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

        /// <summary>
        /// 得到Update()的代码
        /// </summary>
        /// <returns></returns>
        public string CreatUpdate()
        {
            StringBuilder strclass = new StringBuilder();
            StringBuilder strclass2 = new StringBuilder();  //parameters

            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 更新一条数据");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public bool Update(" + _modelname + " model)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "CrDB db = new DBHelper();");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpaceLine(3, "strSql.Append(\"update " + _fieldlist[0].TableName + " set \");");
            

            if (_fieldlist.Count == 0)
            {
                _fieldlist = _keys;
            }

            //主键的字段语句，临时保存
            List<ColumnModel> fieldpk = new List<ColumnModel>();

            strclass2.AppendSpaceLine(3, "List<DBParam> dbParams = new List<DBParam>(new DBParam[] ");
            strclass2.AppendSpaceLine(4, "{");
            foreach (ColumnModel field in _fieldlist)
            {
                if (field.IsIdentity || field.IsPrimaryKey || (_keys.Contains(field)))
                {
                    fieldpk.Add(field);
                    continue;
                }
                if (field.TypeName.ToLower() == "timestamp")
                {
                    continue;
                }
                strclass.AppendSpaceLine(3, "strSql.Append(\"" + field.ColumnName + "=" + _preParameter + field.ColumnName + ",\");");
                strclass2.AppendSpaceLine(5, $"new DBParam(\"{_preParameter + field.ColumnName}\",model.{field.ColumnName.ToFirstUpper()}, DbType.{Win.Common.CodeCommon.SqlTypeToDbType(field.TypeName)},{field.Length}),");

            }

            foreach (ColumnModel field in fieldpk)
            {
                strclass2.AppendSpaceLine(5, $"new DBParam(\"{_preParameter + field.ColumnName}\",model.{field.ColumnName.ToFirstUpper()}, DbType.{Win.Common.CodeCommon.SqlTypeToDbType(field.TypeName)}{(field.Length == 0 ? "" : ", " + field.Length)}),");
            }

            if (strclass2.Length > 0)
            {
                //去掉最后的逗号
                strclass2.DelLastChar(",");
            }
            else
            {
                strclass2.AppendLine("#warning 系统发现缺少更新的字段，请手工确认如此更新是否正确！ ");
                foreach (ColumnModel field in _fieldlist)
                {
                    string columnName = field.ColumnName;
                    strclass2.AppendSpaceLine(3, "strSql.Append(\"" + columnName + "=" + _preParameter + columnName + ",\");");
                }
                if (_fieldlist.Count > 0)
                {
                    strclass2.DelLastChar(",");
                }

            }
            strclass2.AppendLine("});");
            strclass.DelLastChar(",").AppendLine("\"); ");
            strclass.AppendSpaceLine(3, "strSql.Append(\" where " + _identityKey + "=@" + _identityKey + "\");");

            strclass.AppendLine(strclass2.ToString());

            strclass.AppendSpaceLine(3, "int rows=" + _dbhelperName + ".ExecuteNonQuery(strSql.ToString(),dbParams);");

            strclass.AppendSpaceLine(3, "if (rows > 0)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return true;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "else");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return false;");
            strclass.AppendSpaceLine(3, "}");

            strclass.AppendSpaceLine(2, "}");
            return strclass.ToString();
        }

        /// <summary>
        /// 得到Delete的代码
        /// </summary>
        /// <returns></returns>
        public string CreatDelete()
        {
            StringBuilder strclass = new StringBuilder();

            #region 标识字段优先-删除
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 删除一条数据");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public bool Delete(" + _identityKeyType + " " + _identityKey.ToFirstLower() + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, _keysNullTip);
            strclass.AppendSpaceLine(3, "CrDB db = new DBHelper();");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpaceLine(3, "strSql.Append(\"delete from " + _fieldlist[0].TableName + " \");");
            strclass.AppendSpaceLine(3, "strSql.Append(\" where " + _identityKey + "=@" + _identityKey + "\");");
            strclass.AppendSpaceLine(3, "List<DBParam> dbParams = new List<DBParam>(new DBParam[]  {");
            strclass.AppendSpaceLine(4, $"new DBParam(\"@{_identityKey}\",{_identityKey.ToFirstLower()}, DbType.{Win.Common.CodeCommon.SqlTypeToDbType(_identityKeyType)},4)");
            strclass.AppendSpaceLine(3, "});");
            strclass.AppendSpaceLine(3, "int rows=" + _dbhelperName + ".ExecuteNonQuery(strSql.ToString(),dbParams);");
            strclass.AppendSpaceLine(3, "if (rows > 0)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return true;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "else");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return false;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");

            #endregion

            #region 批量删除方法

            string keyField = "";
            if (_keys.Count == 1)
            {
                keyField = _keys[0].ColumnName;
            }
            else
            {
                foreach (ColumnModel field in _keys)
                {
                    if (field.IsIdentity)
                    {
                        keyField = field.ColumnName;
                        break;
                    }
                }
            }
            if (keyField.Trim().Length > 0)
            {
                strclass.AppendSpaceLine(2, "/// <summary>");
                strclass.AppendSpaceLine(2, "/// 批量删除数据");
                strclass.AppendSpaceLine(2, "/// </summary>");
                strclass.AppendSpaceLine(2, "public bool DeleteList(string " + keyField.ToFirstLower() + "list )");
                strclass.AppendSpaceLine(2, "{");
                strclass.AppendSpaceLine(3, "CrDB db = new DBHelper();");
                strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
                strclass.AppendSpaceLine(3, "strSql.Append(\"delete from " + _fieldlist[0].TableName + " \");");
                strclass.AppendSpaceLine(3, "strSql.Append(\" where " + keyField + " in (\"+" + keyField.ToFirstLower() + "list + \")  \");");
                strclass.AppendSpaceLine(3, "int rows=" + _dbhelperName + ".ExecuteNonQuery(strSql.ToString());");
                strclass.AppendSpaceLine(3, "if (rows > 0)");
                strclass.AppendSpaceLine(3, "{");
                strclass.AppendSpaceLine(4, "return true;");
                strclass.AppendSpaceLine(3, "}");
                strclass.AppendSpaceLine(3, "else");
                strclass.AppendSpaceLine(3, "{");
                strclass.AppendSpaceLine(4, "return false;");
                strclass.AppendSpaceLine(3, "}");
                strclass.AppendSpaceLine(2, "}");
            }
            #endregion

            return strclass.ToString();
        }

        /// <summary>
        /// 得到GetModel()的代码
        /// </summary>
        /// <returns></returns>
        public string CreatGetModel()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 得到一个对象实体");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public " + _modelname + " GetModel(" + _identityKeyType + " " + _identityKey.ToFirstLower() + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, _keysNullTip);
            strclass.AppendSpaceLine(3, "CrDB db = new DBHelper();");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpace(3, "strSql.Append(\"select ");

            strclass.Append(" top 1 ");

            strclass.AppendLine(_fieldstrlist + " from " + _fieldlist[0].TableName + " \");");
            strclass.AppendSpaceLine(3, "strSql.Append(\" where " + _identityKey + "=@" + _identityKey + "\");");

            strclass.AppendSpaceLine(3, "List<DBParam> dbParams = new List<DBParam>(new DBParam[]  {");
            strclass.AppendSpaceLine(4, $"new DBParam(\"@{_identityKey}\",{_identityKey.ToFirstLower()}, DbType.{Win.Common.CodeCommon.SqlTypeToDbType(_identityKeyType)},4)");

            strclass.AppendSpaceLine(3, "});");

            //strclass.AppendSpaceLine(3, "" + _modelname + " model=new " + _modelname + "();");
            strclass.AppendSpaceLine(3, "DataSet ds=" + _dbhelperName + ".ExecuteDataSet(strSql.ToString(),dbParams);");
            strclass.AppendSpaceLine(3, "if(ds.Tables[0].Rows.Count>0)");
            strclass.AppendSpaceLine(3, "{");

            strclass.AppendSpaceLine(4, $"return ds.Tables[0].Rows[0].ToEntity<{_modelname}>();");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "else");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return null;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");
            return strclass.ToString();
        }

        ///// <summary>
        ///// DataRowToModel的代码
        ///// </summary>
        //public string CreatDataRowToModel()
        //{
        //    StringBuilder strclass = new StringBuilder();
        //    strclass.AppendLine();
        //    strclass.AppendSpaceLine(2, "/// <summary>");
        //    strclass.AppendSpaceLine(2, "/// 得到一个对象实体");
        //    strclass.AppendSpaceLine(2, "/// </summary>");
        //    strclass.AppendSpaceLine(2, "public " + _modelname + " DataRowToModel(DataRow row)");
        //    strclass.AppendSpaceLine(2, "{");
        //    strclass.AppendSpaceLine(3, "" + _modelname + " model=new " + _modelname + "();");

        //    strclass.AppendSpaceLine(3, "if (row != null)");
        //    strclass.AppendSpaceLine(3, "{");

        //    #region 字段赋值
        //    foreach (ColumnModel field in _fieldlist)
        //    {
        //        string columnName = field.ColumnName;
        //        string columnType = field.TypeName;

        //        //strclass.AppendSpaceLine(4, "if(row[\"" + columnName + "\"]!=null && row[\"" + columnName + "\"].ToString()!=\"\")");
        //        //strclass.AppendSpaceLine(4, "{");
        //        #region
        //        switch (Win.Common.CodeCommon.DbTypeToCS(columnType))
        //        {
        //            case "int":
        //                {
        //                    strclass.AppendSpaceLine(4, "if(row[\"" + columnName + "\"]!=null && row[\"" + columnName + "\"].ToString()!=\"\")");
        //                    strclass.AppendSpaceLine(4, "{");
        //                    strclass.AppendSpaceLine(5, "model." + columnName + "=int.Parse(row[\"" + columnName + "\"].ToString());");
        //                    strclass.AppendSpaceLine(4, "}");
        //                }
        //                break;
        //            case "long":
        //                {
        //                    strclass.AppendSpaceLine(4, "if(row[\"" + columnName + "\"]!=null && row[\"" + columnName + "\"].ToString()!=\"\")");
        //                    strclass.AppendSpaceLine(4, "{");
        //                    strclass.AppendSpaceLine(5, "model." + columnName + "=long.Parse(row[\"" + columnName + "\"].ToString());");
        //                    strclass.AppendSpaceLine(4, "}");
        //                }
        //                break;
        //            case "decimal":
        //                {
        //                    strclass.AppendSpaceLine(4, "if(row[\"" + columnName + "\"]!=null && row[\"" + columnName + "\"].ToString()!=\"\")");
        //                    strclass.AppendSpaceLine(4, "{");
        //                    strclass.AppendSpaceLine(5, "model." + columnName + "=decimal.Parse(row[\"" + columnName + "\"].ToString());");
        //                    strclass.AppendSpaceLine(4, "}");
        //                }
        //                break;
        //            case "float":
        //                {
        //                    strclass.AppendSpaceLine(4, "if(row[\"" + columnName + "\"]!=null && row[\"" + columnName + "\"].ToString()!=\"\")");
        //                    strclass.AppendSpaceLine(4, "{");
        //                    strclass.AppendSpaceLine(5, "model." + columnName + "=float.Parse(row[\"" + columnName + "\"].ToString());");
        //                    strclass.AppendSpaceLine(4, "}");
        //                }
        //                break;
        //            case "DateTime":
        //                {
        //                    strclass.AppendSpaceLine(4, "if(row[\"" + columnName + "\"]!=null && row[\"" + columnName + "\"].ToString()!=\"\")");
        //                    strclass.AppendSpaceLine(4, "{");
        //                    strclass.AppendSpaceLine(5, "model." + columnName + "=DateTime.Parse(row[\"" + columnName + "\"].ToString());");
        //                    strclass.AppendSpaceLine(4, "}");
        //                }
        //                break;
        //            case "string":
        //                {
        //                    strclass.AppendSpaceLine(4, "if(row[\"" + columnName + "\"]!=null)");
        //                    strclass.AppendSpaceLine(4, "{");
        //                    strclass.AppendSpaceLine(5, "model." + columnName + "=row[\"" + columnName + "\"].ToString();");
        //                    strclass.AppendSpaceLine(4, "}");
        //                }
        //                break;
        //            case "bool":
        //                {
        //                    strclass.AppendSpaceLine(4, "if(row[\"" + columnName + "\"]!=null && row[\"" + columnName + "\"].ToString()!=\"\")");
        //                    strclass.AppendSpaceLine(4, "{");
        //                    strclass.AppendSpaceLine(5, "if((row[\"" + columnName + "\"].ToString()==\"1\")||(row[\"" + columnName + "\"].ToString().ToLower()==\"true\"))");
        //                    strclass.AppendSpaceLine(5, "{");
        //                    strclass.AppendSpaceLine(6, "model." + columnName + "=true;");
        //                    strclass.AppendSpaceLine(5, "}");
        //                    strclass.AppendSpaceLine(5, "else");
        //                    strclass.AppendSpaceLine(5, "{");
        //                    strclass.AppendSpaceLine(6, "model." + columnName + "=false;");
        //                    strclass.AppendSpaceLine(5, "}");
        //                    strclass.AppendSpaceLine(4, "}");
        //                }
        //                break;
        //            case "byte[]":
        //                {
        //                    strclass.AppendSpaceLine(4, "if(row[\"" + columnName + "\"]!=null && row[\"" + columnName + "\"].ToString()!=\"\")");
        //                    strclass.AppendSpaceLine(4, "{");
        //                    strclass.AppendSpaceLine(5, "model." + columnName + "=(byte[])row[\"" + columnName + "\"];");
        //                    strclass.AppendSpaceLine(4, "}");
        //                }
        //                break;
        //            case "uniqueidentifier":
        //            case "Guid":
        //                {
        //                    strclass.AppendSpaceLine(4, "if(row[\"" + columnName + "\"]!=null && row[\"" + columnName + "\"].ToString()!=\"\")");
        //                    strclass.AppendSpaceLine(4, "{");
        //                    strclass.AppendSpaceLine(5, "model." + columnName + "= new Guid(row[\"" + columnName + "\"].ToString());");
        //                    strclass.AppendSpaceLine(4, "}");
        //                }
        //                break;
        //            default:
        //                strclass.AppendSpaceLine(5, "//model." + columnName + "=row[\"" + columnName + "\"].ToString();");
        //                break;
        //        }
        //        #endregion
        //        //strclass.AppendSpaceLine(4, "}");
        //    }
        //    #endregion

        //    strclass.AppendSpaceLine(3, "}");
        //    strclass.AppendSpaceLine(3, "return model;");
        //    strclass.AppendSpaceLine(2, "}");
        //    return strclass.ToString();
        //}

        /// <summary>
        /// 得到GetList()的代码
        /// </summary>
        /// <returns></returns>
        public string CreatGetList()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 获得数据列表");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetList(string strWhere)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "CrDB db = new DBHelper();");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpace(3, "strSql.Append(\"select ");
            strclass.AppendLine(_fieldstrlist + " \");");
            strclass.AppendSpaceLine(3, "strSql.Append(\" FROM " + _fieldlist[0].TableName + " \");");
            strclass.AppendSpaceLine(3, "if(strWhere.Trim()!=\"\")");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "strSql.Append(\" where \"+strWhere);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "return " + _dbhelperName + ".ExecuteDataSet(strSql.ToString());");
            strclass.AppendSpaceLine(2, "}");

            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 获得前几行数据");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetList(string strWhere,string filedOrder)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "CrDB db = new DBHelper();");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpaceLine(3, "strSql.Append(\"select \");");
            strclass.AppendSpaceLine(3, "strSql.Append(\" " + _fieldstrlist + " \");");
            strclass.AppendSpaceLine(3, "strSql.Append(\" FROM " + _fieldlist[0].TableName + " \");");
            strclass.AppendSpaceLine(3, "if(strWhere.Trim()!=\"\")");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "strSql.Append(\" where \"+strWhere);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "strSql.Append(\" order by \" + filedOrder);");
            strclass.AppendSpaceLine(3, "return " + _dbhelperName + ".ExecuteDataSet(strSql.ToString());");
            strclass.AppendSpaceLine(2, "}");

            strclass.AppendLine();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 获得前几行数据");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetList(int top,string strWhere,string filedOrder)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "CrDB db = new DBHelper();");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpaceLine(3, "strSql.Append(\"select \");");
            strclass.AppendSpaceLine(3, "if(top>0)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "strSql.Append(\" top \"+top);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "strSql.Append(\" " + _fieldstrlist + " \");");
            strclass.AppendSpaceLine(3, "strSql.Append(\" FROM " + _fieldlist[0].TableName + " \");");
            strclass.AppendSpaceLine(3, "if(strWhere.Trim()!=\"\")");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "strSql.Append(\" where \"+strWhere);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "strSql.Append(\" order by \" + filedOrder);");
            strclass.AppendSpaceLine(3, "return " + _dbhelperName + ".ExecuteDataSet(strSql.ToString());");
            strclass.AppendSpaceLine(2, "}");


            return strclass.ToString();
        }


        /// <summary>
        /// 得到分页方法的代码
        /// </summary>
        public string CreatGetListByPage()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 获取记录总数");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public int GetRecordCount(string strWhere)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "CrDB db = new DBHelper();");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpaceLine(3, "strSql.Append(\"select count(1) FROM " + _fieldlist[0].TableName + " \");");
            strclass.AppendSpaceLine(3, "if(strWhere.Trim()!=\"\")");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "strSql.Append(\" where \"+strWhere);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "object obj = "+ _dbhelperName + ".ExecuteScalar(strSql.ToString());");
            strclass.AppendSpaceLine(3, "if (obj == null)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return 0;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "else");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return Convert.ToInt32(obj);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");



            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 分页获取数据列表");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "CrDB db = new DBHelper();");
            strclass.AppendSpaceLine(3, "StringBuilder strSql=new StringBuilder();");
            strclass.AppendSpaceLine(3, "strSql.Append(\"SELECT * FROM ( \");");
            strclass.AppendSpaceLine(3, "strSql.Append(\" SELECT ROW_NUMBER() OVER (\");");
            strclass.AppendSpaceLine(3, "if (!string.IsNullOrEmpty(orderby.Trim()))");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "strSql.Append(\"order by T.\" + orderby );");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "else");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "strSql.Append(\"order by T." + _identityKey + " desc\");");
            strclass.AppendSpaceLine(3, "}");

            strclass.AppendSpaceLine(3, "strSql.Append(\")AS Row, T.*  from " + _fieldlist[0].TableName + " T \");");
            strclass.AppendSpaceLine(3, "if (!string.IsNullOrEmpty(strWhere.Trim()))");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "strSql.Append(\" WHERE \" + strWhere);");
            strclass.AppendSpaceLine(3, "}");

            strclass.AppendSpaceLine(3, "strSql.Append(\" ) TT\");");
            strclass.AppendSpaceLine(3, "strSql.AppendFormat(\" WHERE TT.Row between {0} and {1}\", startIndex, endIndex);");

            strclass.AppendSpaceLine(3, "return " + _dbhelperName + ".ExecuteDataSet(strSql.ToString());");
            strclass.AppendSpaceLine(2, "}");


            return strclass.ToString();
        }


        /// <summary>
        /// 得到GetList()的代码
        /// </summary>
        /// <returns></returns>
        public string CreatGetListByPageProc()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendSpaceLine(2, "/*");
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 分页获取数据列表");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetList(int PageSize,int PageIndex,string strWhere)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "" + _dbParaHead + "Parameter[] parameters = {");
            strclass.AppendSpaceLine(5, "new " + _dbParaHead + "Parameter(\"" + _preParameter + "tblName\", SqlDbType.VarChar, 255),");
            strclass.AppendSpaceLine(5, "new " + _dbParaHead + "Parameter(\"" + _preParameter + "fldName\", SqlDbType.VarChar, 255),");
            strclass.AppendSpaceLine(5, "new " + _dbParaHead + "Parameter(\"" + _preParameter + "PageSize\", SqlDbType.Int),");
            strclass.AppendSpaceLine(5, "new " + _dbParaHead + "Parameter(\"" + _preParameter + "PageIndex\", SqlDbType.Int),");
            strclass.AppendSpaceLine(5, "new " + _dbParaHead + "Parameter(\"" + _preParameter + "IsReCount\", SqlDbType.Bit),");
            strclass.AppendSpaceLine(5, "new " + _dbParaHead + "Parameter(\"" + _preParameter + "OrderType\", SqlDbType.Bit),");
            strclass.AppendSpaceLine(5, "new " + _dbParaHead + "Parameter(\"" + _preParameter + "strWhere\", SqlDbType.VarChar,1000),");
            strclass.AppendSpaceLine(5, "};");
            strclass.AppendSpaceLine(3, "parameters[0].Value = \"" + this._fieldlist[0].TableName + "\";");
            strclass.AppendSpaceLine(3, "parameters[1].Value = \"" + this._identityKey + "\";");
            strclass.AppendSpaceLine(3, "parameters[2].Value = PageSize;");
            strclass.AppendSpaceLine(3, "parameters[3].Value = PageIndex;");
            strclass.AppendSpaceLine(3, "parameters[4].Value = 0;");
            strclass.AppendSpaceLine(3, "parameters[5].Value = 0;");
            strclass.AppendSpaceLine(3, "parameters[6].Value = strWhere;	");
            strclass.AppendSpaceLine(3, "return " + _dbhelperName + ".RunProcedure(\"UP_GetRecordByPage\",parameters,\"ds\");");
            strclass.AppendSpaceLine(2, "}*/");
            return strclass.ToString();
        }

        #endregion


    }
}
