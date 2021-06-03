using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Win.Common.DBHelper;
using Win.Models;

namespace Win.DAL.BLL
{
    public class SQLiteBll
    {
        private static string _connectionString;
        private static string _providerName;
        public SQLiteBll()
        {
            var dataBaseConfig = Win.Models.Config.DataConfig.Instance.GetDataBaseConfig();
            _connectionString = dataBaseConfig.ConnStr;
            _providerName = dataBaseConfig.ProviderName;
        }
        public static DataTable GetAllDatabase()
        {
            try
            {
                CrDB db = new CrDB(_connectionString, _providerName);
                string sql = "Select name from master..sysdatabases";
                return db.ExecuteDataTable(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 获得所有表名
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllTable()
        {
            CrDB db = new CrDB(_connectionString, _providerName);
            string sql = "select name as TABLE_NAME from sqlite_master where type='table' order by name;";
            return db.ExecuteDataTable(sql);
        }
        /// <summary>
        /// 获得字段表
        /// </summary>
        /// <param name="tablename">数据表名</param>
        /// <returns></returns>
        public static DataTable GetColumnTable(string tablename)
        {
            CrDB db = new CrDB(_connectionString, _providerName);
            DataTable datatable = db.ExecuteDataTable($"PRAGMA table_info('{tablename}')");
            DataTable dt = new DataTable();
            dt.Columns.Add("表名");
            dt.Columns.Add("表说明");
            dt.Columns.Add("序号");
            dt.Columns.Add("字段名");
            dt.Columns.Add("标识");
            dt.Columns.Add("主键");
            dt.Columns.Add("类型");
            dt.Columns.Add("字节数");
            dt.Columns.Add("长度");
            dt.Columns.Add("小数");
            dt.Columns.Add("允许空");
            dt.Columns.Add("默认值");
            dt.Columns.Add("说明");
            foreach (DataRow dataRow in datatable.Rows)
            {
                DataRow dr = dt.NewRow();
                dr["表名"] = tablename;
                dr["表说明"] = "";
                dr["序号"] = dataRow["cid"];
                dr["字段名"] = dataRow["name"];
                dr["标识"] = dataRow["pk"].ToString() == "1" ? "√" : "";
                dr["主键"] = dataRow["pk"].ToString() == "1" ? "√" : "";
                string type = Regex.Match(dataRow["type"].ToString(), "[A-Za-z]+").Groups[0].Value;
                dr["类型"] = type;
                string length = Regex.Match(dataRow["type"].ToString(), "\\d+").Groups[0].Value;
                dr["字节数"] = length == "" ? "0" : length;
                dr["长度"] = "0";
                dr["小数"] = "0";
                dr["允许空"] = dataRow["notnull"].ToString() == "0" ? "√" : "";
                dr["默认值"] = dataRow["dflt_value"];
                dr["说明"] = "";
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 获得字段集合
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public static List<ColumnModel> GetTableColumnInfo(string tablename)
        {
            DataTable dataTable = GetColumnTable(tablename);
            List<ColumnModel> fieldlist = new List<ColumnModel>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                ColumnModel field = new ColumnModel();
                field.TableName = dataTable.Rows[0]["表名"].ToString();
                field.TableDescription = dataTable.Rows[0]["表说明"].ToString();
                field.ColumnOrder = dataRow["序号"].ToString();
                field.ColumnName = dataRow["字段名"].ToString();
                field.TypeName = dataRow["类型"].ToString();
                field.Length = int.Parse(dataRow["长度"].ToString());
                field.Precision = int.Parse(dataRow["字节数"].ToString());
                field.Scale = dataRow["小数"].ToString();
                field.IsIdentity = dataRow["标识"].ToString() == "√";
                field.IsPrimaryKey = dataRow["主键"].ToString() == "√";
                field.IsCanNull = dataRow["允许空"].ToString() == "√";
                field.DefaultVal = dataRow["默认值"].ToString();
                field.Description = dataRow["说明"].ToString();
                fieldlist.Add(field);
            }
            return fieldlist;
        }

        /// <summary>
        /// 返回数据表列名
        /// </summary>
        /// <param name="tablename">表名</param>
        /// <returns></returns>
        public static DataColumnCollection Returncolumns(string tablename)
        {
            CrDB db = new CrDB(_connectionString, _providerName);
            DataTable datatable = db.ExecuteDataTable("select top 0 * from " + tablename);
            DataColumnCollection columns = datatable.Columns;
            return columns;
        }
    }
}