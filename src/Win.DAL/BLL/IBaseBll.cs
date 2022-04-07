using System.Data;

namespace Win.DAL.BLL;

public interface IBaseBll
{
    /// <summary>
    /// 获得所有数据表
    /// </summary>
    /// <returns></returns>
    DataTable GetAllTable();
    /// <summary>
    /// 获得字段表
    /// </summary>
    /// <param name="tablename">数据表名</param>
    /// <returns></returns>
    DataTable GetColumnTable(string tablename);
}