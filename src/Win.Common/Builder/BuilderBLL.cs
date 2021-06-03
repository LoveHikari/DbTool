using System;
using System.Collections.Generic;
using System.Text;
using Win.Models;

namespace Win.Common.Builder
{
    /// <summary>
    /// 业务层代码组件
    /// </summary>
    public class BuilderBLL
    {
        #region 字段
        private string _modelname; //model类名
        private string _dalname; //dal类名
        private string _bllname; //bll类名
        private List<ColumnModel> _fieldlist;  //选择要生成的字段集合
        private List<ColumnModel> _keys; // 主键或条件字段列表
        private string _dbhelperName; //数据库访问类名
        private string _bllpath; //业务逻辑层的命名空间
        private string _dalpath; //数据层的命名空间
        private string _modelpath; //model命名空间

        private string _dbParaHead;  //不同数据库类的前缀
        private string _identityKey = "";  //标识列，或主键字段
        private string _identityKeyType = "int";  //标识列，或主键字段类型
        private string _fieldstrlist;  //所选字段的 select 列表
        private string _preParameter;  //存储过程参数 调用符号@
        private bool _isHasIdentity;  //主键或条件字段中是否有标识列
        private string _keysNullTip;
        #endregion

        #region  构造函数
        public BuilderBLL(List<ColumnModel> fieldlist, string modelpath, string dalpath, string bllpath, string modelSuffix, string dalSuffix, string bllSuffix)
        {
            _fieldlist = fieldlist;
            _bllpath = bllpath;
            _modelpath = modelpath;
            _dalpath = dalpath;
            string tableName = _fieldlist[0].TableName;
            _modelname = tableName.ToPascal() + modelSuffix;
            _dalname = tableName.ToPascal() + dalSuffix;
            _bllname = tableName.ToPascal() + bllSuffix;
            _keys = Win.Common.CodeCommon.GetPrimaryKeyList(fieldlist);
            foreach (ColumnModel key in _keys)
            {
                _identityKey = key.ColumnName;
                _identityKeyType = key.TypeName;
                if (key.IsIdentity)
                {
                    _identityKey = key.ColumnName;
                    _identityKeyType = Win.Common.CodeCommon.DbTypeToCS(key.TypeName);
                    break;
                }
            }

            StringBuilder fields = new StringBuilder();
            foreach (ColumnModel obj in _fieldlist)
            {
                fields.Append(obj.ColumnName + ",");
            }
            _fieldstrlist = fields.DelLastChar(",").ToString();

            _preParameter = "@";
            _isHasIdentity = Win.Common.CodeCommon.IsHasIdentity(_keys);
            _keysNullTip = _keys.Count == 0 ? "//该表无主键信息，请自定义主键/条件字段" : "";
        }
        #endregion

        #region 业务层方法
        /// <summary>
        /// 得到整个类的代码
        /// </summary>
        /// <param name="maxid"></param>
        /// <param name="exists"></param>
        /// <param name="add"></param>
        /// <param name="update"></param>
        /// <param name="delete"></param>
        /// <param name="getModel"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public string CreatBll(bool maxid, bool exists, bool add, bool update, bool delete, bool getModel, bool list)
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendLine("using System;");
            strclass.AppendLine("using System.Data;");
            strclass.AppendLine("using System.Collections.Generic;");
            strclass.AppendLine("using " + _dalpath + ";");
            strclass.AppendLine("using " + _modelpath + ";");
            strclass.AppendLine("using Com.Cos.Utility;");
            strclass.AppendLine("namespace " + _bllpath);
            strclass.AppendLine("{");
            strclass.AppendSpaceLine(1, "/// <summary>");

            strclass.AppendSpaceLine(1, "/// " + _fieldlist[0].TableName + ": " + _fieldlist[0].TableDescription);

            strclass.AppendSpaceLine(1, "/// </summary>");
            strclass.AppendSpaceLine(1, "public partial class " + _bllname);
            strclass.AppendSpaceLine(1, "{");

            #region 单例模式代码

            strclass.AppendLine(CreatInstance());

            #endregion

            strclass.AppendSpaceLine(2, "#region  自动生成");
            #region  方法代码
            //if (Maxid)
            //{
            //    if (Keys.Count > 0)
            //    {
            //        foreach (ColumnInfo obj in Keys)
            //        {
            //            if (CodeCommon.DbTypeToCS(obj.TypeName) == "int")
            //            {
            //                if (obj.IsPrimaryKey)
            //                {
            //                    strclass.AppendLine(CreatBLLGetMaxID());
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}
            //if (Exists)
            //{
            //    strclass.AppendLine(CreatBLLExists());
            //}
            if (add)
            {
                strclass.AppendLine(CreatBlladd());
            }
            if (update)
            {
                strclass.AppendLine(CreatBllUpdate());
            }
            if (delete)
            {
                strclass.AppendLine(CreatBllDelete());
            }
            if (getModel)
            {
                strclass.AppendLine(CreatBllGetModel());
            }
            if (list)
            {
                strclass.AppendLine(CreatBllGetList());
                strclass.AppendLine(CreatBllGetAllList());
                strclass.AppendLine(CreatBllGetListByPage());
            }

            #endregion
            strclass.AppendSpaceLine(2, "#endregion");

            strclass.AppendSpaceLine(1, "}");
            strclass.AppendLine("}");
            strclass.AppendLine("");

            return strclass.ToString();
        }

        #endregion
        /// <summary>
        /// 得到单例模式代码
        /// </summary>
        /// <returns></returns>
        private string CreatInstance()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendSpaceLine(2, "#region  instance");
            strclass.AppendSpaceLine(2, "private volatile static " + _bllname + " _instance = null;");
            strclass.AppendSpaceLine(2, "private static readonly object lockHelper = new object();");
            strclass.AppendSpaceLine(2, "public " + _bllname + "(){}");
            strclass.AppendSpaceLine(2, "public static " + _bllname + " Instance");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "get");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "if (_instance == null)");
            strclass.AppendSpaceLine(4, "{");
            strclass.AppendSpaceLine(5, "lock (lockHelper)");
            strclass.AppendSpaceLine(5, "{");
            strclass.AppendSpaceLine(6, "if (_instance == null)");
            strclass.AppendSpaceLine(7, " _instance = new " + _bllname + "();");
            strclass.AppendSpaceLine(5, "}");
            strclass.AppendSpaceLine(4, "}");
            strclass.AppendSpaceLine(4, "return _instance;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");
            strclass.AppendSpaceLine(2, "#endregion");
            return strclass.ToString();
        }

        #region 具体方法代码

        //public string CreatBLLGetMaxID()
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
        //                    strclass.AppendSpaceLine(2, "/// 获得最大ID");
        //                    strclass.AppendSpaceLine(2, "/// </summary>");
        //                    strclass.AppendSpaceLine(2, "public int GetMaxId()");
        //                    strclass.AppendSpaceLine(2, "{");
        //                    strclass.AppendSpaceLine(3, "return dal.GetMaxId();");
        //                    strclass.AppendSpaceLine(2, "}");
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return strclass.ToString();

        //}
        //public string CreatBLLExists()
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
        //            strclass.AppendSpaceLine(3, "return dal.Exists(" + Maticsoft.CodeHelper.CodeCommon.GetFieldstrlist(Keys, false) + ");");
        //            strclass.AppendSpaceLine(2, "}");
        //        }

        //    }
        //    return strclass.ToString();
        //}
        public string CreatBlladd()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 增加一条数据");
            strclass.AppendSpaceLine(2, "/// </summary>");
            string strretu = "bool";
            if (_isHasIdentity)
            {
                strretu = "int ";
                if (_identityKeyType != "int")
                {
                    strretu = _identityKeyType;
                }
            }
            strclass.AppendSpaceLine(2, "public " + strretu + " Add(" + _modelname + " model)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "try");
            strclass.AppendSpaceLine(3, "{");
            if (strretu == "void")
            {
                strclass.AppendSpaceLine(4, _dalname + ".Instance.Add(model);");
            }
            else
            {
                strclass.AppendSpaceLine(4, "return " + _dalname + ".Instance.Add(model);");
            }
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "catch (System.Exception ex)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "WriteLog.WriteError(ex);");
            strclass.AppendSpaceLine(4, "throw ex;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");
            return strclass.ToString();
        }
        public string CreatBllUpdate()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 更新一条数据");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public bool Update(" + _modelname + " model)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "try");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return " + _dalname + ".Instance.Update(model);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "catch (System.Exception ex)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "WriteLog.WriteError(ex);");
            strclass.AppendSpaceLine(4, "throw ex;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");
            return strclass.ToString();
        }
        public string CreatBllDelete()
        {
            StringBuilder strclass = new StringBuilder();

            #region 标识字段优先的删除
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 删除一条数据");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public bool Delete(" + _identityKeyType + " " + _identityKey + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "try");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, _keysNullTip);
            strclass.AppendSpaceLine(4, "return " + _dalname + ".Instance.Delete(" + _identityKey + ");");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "catch (System.Exception ex)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "WriteLog.WriteError(ex);");
            strclass.AppendSpaceLine(4, "throw ex;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");

            #endregion

            #region 批量删除

            //批量删除方法
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
                strclass.AppendSpaceLine(2, "/// 删除一条数据");
                strclass.AppendSpaceLine(2, "/// </summary>");
                strclass.AppendSpaceLine(2, "public bool DeleteList(string " + keyField + "list )");
                strclass.AppendSpaceLine(2, "{");
                strclass.AppendSpaceLine(3, "try");
                strclass.AppendSpaceLine(3, "{");
                strclass.AppendSpaceLine(4, "return " + _dalname + ".Instance.DeleteList(" + keyField + "list );");
                strclass.AppendSpaceLine(3, "}");
                strclass.AppendSpaceLine(3, "catch (System.Exception ex)");
                strclass.AppendSpaceLine(3, "{");
                strclass.AppendSpaceLine(4, "WriteLog.WriteError(ex);");
                strclass.AppendSpaceLine(4, "throw ex;");
                strclass.AppendSpaceLine(3, "}");
                strclass.AppendSpaceLine(2, "}");
            }
            #endregion

            return strclass.ToString();
        }
        public string CreatBllGetModel()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 得到一个对象实体");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public " + _modelname + " GetModel(" + _identityKeyType + " " + _identityKey + ")");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "try");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, _keysNullTip);
            strclass.AppendSpaceLine(4, "return " + _dalname + ".Instance.GetModel(" + _identityKey + ");");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "catch (System.Exception ex)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "WriteLog.WriteError(ex);");
            strclass.AppendSpaceLine(4, "throw ex;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");

            return strclass.ToString();

        }
        public string CreatBllGetList()
        {
            StringBuilder strclass = new StringBuilder();
            //返回DataSet
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 获得数据列表");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetList(string strWhere)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "try");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return " + _dalname + ".Instance.GetList(strWhere);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "catch (System.Exception ex)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "WriteLog.WriteError(ex);");
            strclass.AppendSpaceLine(4, "throw ex;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");
            //返回DataSet
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 获得数据列表");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetList(string strWhere,string filedOrder)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "try");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return " + _dalname + ".Instance.GetList(strWhere,filedOrder);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "catch (System.Exception ex)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "WriteLog.WriteError(ex);");
            strclass.AppendSpaceLine(4, "throw ex;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");

            //返回DataSet
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 获得前几行数据");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetList(int Top,string strWhere,string filedOrder)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "try");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return " + _dalname + ".Instance.GetList(Top,strWhere,filedOrder);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "catch (System.Exception ex)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "WriteLog.WriteError(ex);");
            strclass.AppendSpaceLine(4, "throw ex;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");



            //返回List<>
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 获得数据列表");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public List<" + _modelname + "> GetModelList(string strWhere)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "try");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "DataSet ds = " + _dalname + ".Instance.GetList(strWhere);");
            strclass.AppendSpaceLine(4, "return DataTableToList(ds.Tables[0]);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "catch (System.Exception ex)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "WriteLog.WriteError(ex);");
            strclass.AppendSpaceLine(4, "throw ex;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");


            //返回List<>
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 获得数据列表");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public List<" + _modelname + "> DataTableToList(DataTable dt)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "try");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(3, "List<" + _modelname + "> modelList = new List<" + _modelname + ">();");
            strclass.AppendSpaceLine(3, "int rowsCount = dt.Rows.Count;");
            strclass.AppendSpaceLine(3, "if (rowsCount > 0)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, _modelname + " model;");
            strclass.AppendSpaceLine(4, "for (int n = 0; n < rowsCount; n++)");
            strclass.AppendSpaceLine(4, "{");

            strclass.AppendSpaceLine(5, "model = " + _dalname + ".Instance.DataRowToModel(dt.Rows[n]);");
            strclass.AppendSpaceLine(5, "if (model != null)");
            strclass.AppendSpaceLine(5, "{");
            strclass.AppendSpaceLine(6, "modelList.Add(model);");
            strclass.AppendSpaceLine(5, "}");

            strclass.AppendSpaceLine(4, "}");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "return modelList;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "catch (System.Exception ex)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "WriteLog.WriteError(ex);");
            strclass.AppendSpaceLine(4, "throw ex;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");



            return strclass.ToString();

        }
        public string CreatBllGetAllList()
        {
            StringBuilder strclass = new StringBuilder();
            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 获得数据列表");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetAllList()");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "try");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return GetList(\"\");");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "catch (System.Exception ex)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "WriteLog.WriteError(ex);");
            strclass.AppendSpaceLine(4, "throw ex;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");
            return strclass.ToString();
        }
        public string CreatBllGetListByPage()
        {
            StringBuilder strclass = new StringBuilder();

            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 分页获取数据列表");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public int GetRecordCount(string strWhere)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "try");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return " + _dalname + ".Instance.GetRecordCount(strWhere);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "catch (System.Exception ex)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "WriteLog.WriteError(ex);");
            strclass.AppendSpaceLine(4, "throw ex;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");

            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 分页获取数据列表");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)");
            strclass.AppendSpaceLine(2, "{");
            strclass.AppendSpaceLine(3, "try");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "return " + _dalname + ".Instance.GetListByPage( strWhere,  orderby,  startIndex,  endIndex);");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(3, "catch (System.Exception ex)");
            strclass.AppendSpaceLine(3, "{");
            strclass.AppendSpaceLine(4, "WriteLog.WriteError(ex);");
            strclass.AppendSpaceLine(4, "throw ex;");
            strclass.AppendSpaceLine(3, "}");
            strclass.AppendSpaceLine(2, "}");


            strclass.AppendSpaceLine(2, "/// <summary>");
            strclass.AppendSpaceLine(2, "/// 分页获取数据列表");
            strclass.AppendSpaceLine(2, "/// </summary>");
            strclass.AppendSpaceLine(2, "//public DataSet GetList(int PageSize,int PageIndex,string strWhere)");
            strclass.AppendSpaceLine(2, "//{");
            strclass.AppendSpaceLine(3, "//return " + _dalname + ".Instance.GetList(PageSize,PageIndex,strWhere);");
            strclass.AppendSpaceLine(2, "//}");
            return strclass.ToString();
        }

        #endregion


    }
}
