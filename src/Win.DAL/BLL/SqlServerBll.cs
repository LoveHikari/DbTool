using Hikari.DbCore;
using System.Collections.Generic;
using System.Data;
using Win.Models;

namespace Win.DAL.BLL
{
    public class SqlServerBll : IBaseBll
    {
        private static string _connectionString;

        public SqlServerBll(string connectionString)
        {
            _connectionString = connectionString;
        }
        /// <summary>
        /// 获得所有数据库
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllDatabase()
        {
            CrDb db = new CrDb(_connectionString);
            string sql = "Select name from master..sysdatabases  ORDER BY name";
            return db.ExecuteDataTable(sql);

        }
        /// <summary>
        /// 获得所有数据表
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllTable()
        {
            CrDb db = new CrDb(_connectionString);
            string sql = "SELECT TABLE_NAME as TableName FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' ORDER BY TABLE_NAME";
            return db.ExecuteDataTable(sql);
        }
        /// <summary>
        /// 获得字段表
        /// </summary>
        /// <param name="tablename">数据表名</param>
        /// <returns></returns>
        public DataTable GetColumnTable(string tablename)
        {
            CrDb db = new CrDb(_connectionString);
            DataTable datatable = db.ExecuteDataTable($@"SELECT 
                                                TableName   = CASE a.colorder WHEN 1 THEN c.name ELSE '' END, 
                                                TableDescription = CASE a.colorder WHEN 1 THEN isnull(g.value,'') ELSE '' END,
                                                ColumnOrder     = a.colorder, 
                                                ColumnName = a.name, 
                                                IsIdentity   = CASE COLUMNPROPERTY(a.id,a.name,'IsIdentity') WHEN 1 THEN 'true' ELSE 'false' END, 
                                                IsPrimaryKey   = CASE 
                                                WHEN EXISTS ( 
                                                SELECT * 
                                                FROM sysobjects 
                                                WHERE xtype='PK' AND name IN ( 
                                                SELECT name 
                                                FROM sysindexes 
                                                WHERE id=a.id AND indid IN ( 
                                                SELECT indid 
                                                FROM sysindexkeys 
                                                WHERE id=a.id AND colid IN ( 
                                                SELECT colid 
                                                FROM syscolumns 
                                                WHERE id=a.id AND name=a.name 
                                                ) 
                                                ) 
                                                ) 
                                                ) 
                                                THEN 'true' 
                                                ELSE 'false' 
                                                END, 
                                                TypeName   = b.name, 
                                                Precision = a.length, 
                                                Length   = COLUMNPROPERTY(a.id,a.name,'Precision'), 
                                                Scale   = CASE ISNULL(COLUMNPROPERTY(a.id,a.name,'Scale'),0) 
                                                WHEN 0 THEN '' 
                                                ELSE CAST(COLUMNPROPERTY(a.id,a.name,'Scale') AS VARCHAR) 
                                                END, 
                                                IsCanNull = CASE a.isnullable WHEN 1 THEN 'true' ELSE 'false' END, 
                                                DefaultVal = ISNULL(d.[text],''), 
                                                Description   = ISNULL(e.[value],'') 
                                                FROM syscolumns a 
                                                LEFT  JOIN systypes      b ON a.xtype=b.xusertype 
                                                INNER JOIN sysobjects    c ON a.id=c.id AND c.xtype='U' AND c.name<>'dtproperties' 
                                                LEFT  JOIN syscomments   d ON a.cdefault=d.id 
                                                LEFT  JOIN sys.extended_properties e ON a.id=e.major_id AND a.colid=e.minor_id 
                                                LEFT JOIN sys.extended_properties g ON (a.id=g.major_id AND  g.minor_id = 0 )
                                                WHERE c.name = '{tablename}'
                                                ORDER BY c.name, a.colorder");
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
            CrDb db = new CrDb(_connectionString);
            DataTable datatable = db.ExecuteDataTable("select top 0 * from " + tablename);
            DataColumnCollection columns = datatable.Columns;
            return columns;
        }
    }
}