using System;
using System.Collections.Generic;
using System.Data;
using Hikari.DbCore;
using Win.Common.DBHelper;
using Win.Models;

namespace Win.DAL.BLL
{
    public class MySqlBll
    {
        #region instance
        private static volatile MySqlBll _instance = null;
        private static readonly object lockHelper = new object();
        public static MySqlBll Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockHelper)
                    {
                        if (_instance == null)
                            _instance = new MySqlBll();
                    }
                }
                return _instance;
            }
        }
        #endregion
        private static string _connectionString;
        private static string _providerName;
        public MySqlBll()
        {
            var dataBaseConfig = Win.Models.Config.DataConfig.Instance.GetDataBaseConfig();
            _connectionString = dataBaseConfig.ConnStr;
            _providerName = dataBaseConfig.ProviderName;
        }
        /// <summary>
        /// 获得所有数据库
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllDatabase()
        {
            try
            {
                CrDb db = new CrDb(_connectionString, _providerName);
                string sql = "SHOW DATABASES";
                return db.ExecuteDataTable(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        /// <summary>
        /// 获得所有数据表
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllTable()
        {
            CrDB db = new CrDB(_connectionString, _providerName);
            string sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' ORDER BY TABLE_NAME";
            return db.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 获得字段表
        /// </summary>
        /// <param name="schema">数据库名</param>
        /// <param name="tablename">数据表名</param>
        /// <returns></returns>
        public DataTable GetColumnTable(string schema, string tablename)
        {
            CrDB db = new CrDB(_connectionString, _providerName);
            DataTable datatable = db.ExecuteDataTable($@"SELECT a.table_schema AS '表名',b.TABLE_COMMENT AS '表说明',a.ordinal_position AS '序号',a.column_name AS '字段名',
                                CASE a.extra WHEN '' THEN '' ELSE '√' END AS '标识',
                                CASE a.column_key WHEN '' THEN '' ELSE '√' END AS '主键',
                                a.DATA_TYPE AS '类型',
                                IFNULL(a.CHARACTER_OCTET_LENGTH,'0')AS '字节数',
                                IFNULL(a.CHARACTER_MAXIMUM_LENGTH,'0')AS '长度',
                                IFNULL(a.NUMERIC_SCALE,'')AS '小数',
                                CASE a.IS_NULLABLE WHEN 'NO' THEN '' ELSE '√' END AS '允许空',
                                IFNULL(a.COLUMN_DEFAULT,'') AS '默认值',
                                a.COLUMN_COMMENT AS '说明'
                                FROM information_schema.COLUMNS  AS a
                                LEFT  JOIN  information_schema.tables  AS b ON a.TABLE_SCHEMA=b.TABLE_SCHEMA AND a.TABLE_NAME=b.TABLE_NAME
                                WHERE a.table_schema = '{schema}' AND a.table_name = '{tablename}';");
            return datatable;
        }

        /// <summary>
        /// 获得字段集合
        /// </summary>
        /// <param name="schema">数据库名</param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public List<ColumnModel> GetTableColumnInfo(string schema, string tablename)
        {
            DataTable dataTable = GetColumnTable(schema,tablename);
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
        public DataColumnCollection Returncolumns(string tablename)
        {
            CrDB db = new CrDB(_connectionString, _providerName);
            DataTable datatable = db.ExecuteDataTable("select top 0 * from " + tablename);
            DataColumnCollection columns = datatable.Columns;
            return columns;
        }
    }
}