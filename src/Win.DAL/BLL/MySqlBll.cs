using Hikari.DbCore;
using System;
using System.Collections.Generic;
using System.Data;
using Win.Models;

namespace Win.DAL.BLL
{
    public class MySqlBll : IBaseBll
    {
        private string _schema;  // 数据库名
        private CrDb _db;
        public MySqlBll(string connectionString)
        {
            string[] ss = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in ss)
            {
                var vs = s.Split('=');
                if (vs[0].ToLower().Trim() == "Database".ToLower())
                {
                    _schema = vs[1];
                    break;
                }
            }
            _db = new CrDb(connectionString, DbProviderEnum.MySql);
        }
        /// <summary>
        /// 获得所有数据库
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllDatabase()
        {
            string sql = "SHOW DATABASES";
            return _db.ExecuteDataTable(sql);

        }
        /// <summary>
        /// 获得所有数据表
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllTable()
        {
            string sql = $"SELECT TABLE_NAME as TableName FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' and TABLE_SCHEMA='{_schema}' ORDER BY TABLE_NAME";
            return _db.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 获得字段表
        /// </summary>
        /// <param name="tablename">数据表名</param>
        /// <returns></returns>
        public DataTable GetColumnTable(string tablename)
        {
            DataTable datatable = _db.ExecuteDataTable($@"SELECT b.TABLE_NAME AS 'TableName',b.TABLE_COMMENT AS 'TableDescription',a.ordinal_position AS 'ColumnOrder',a.column_name AS 'ColumnName',
                                CASE a.extra WHEN '' THEN 'false' ELSE 'true' END AS 'IsIdentity',
                                CASE a.column_key WHEN '' THEN 'false' ELSE 'true' END AS 'IsPrimaryKey',
                                a.DATA_TYPE AS 'TypeName',
                                IFNULL(a.CHARACTER_OCTET_LENGTH,'0')AS 'Precision',
                                IFNULL(a.CHARACTER_MAXIMUM_LENGTH,'0')AS 'Length',
                                IFNULL(a.NUMERIC_SCALE,'')AS 'Scale',
                                CASE a.IS_NULLABLE WHEN 'NO' THEN 'false' ELSE 'true' END AS 'IsCanNull',
                                IFNULL(a.COLUMN_DEFAULT,'') AS 'DefaultVal',
                                a.COLUMN_COMMENT AS 'Description'
                                FROM information_schema.COLUMNS  AS a
                                LEFT  JOIN  information_schema.tables  AS b ON a.TABLE_SCHEMA=b.TABLE_SCHEMA AND a.TABLE_NAME=b.TABLE_NAME
                                WHERE a.table_schema = '{_schema}' AND a.table_name = '{tablename}';");
            return datatable;
        }

        /// <summary>
        /// 获得字段集合
        /// </summary>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public List<ColumnModel> GetTableColumnInfo(string tablename)
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
        public DataColumnCollection Returncolumns(string tablename)
        {
            DataTable datatable = _db.ExecuteDataTable("select top 0 * from " + tablename);
            DataColumnCollection columns = datatable.Columns;
            return columns;
        }
    }
}