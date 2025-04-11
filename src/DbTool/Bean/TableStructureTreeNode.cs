using Win.Models;

namespace DbTool.Bean;
/// <summary>
/// 数据库表结构树节点
/// </summary>
public class TableStructureTreeNode
{
    /// <summary>
    /// 表名
    /// </summary>
    public string ColumnName { get; set; }
    /// <summary>
    /// 表说明/表描述
    /// </summary>
    public string Description { get; set; }
    public List<ColumnModel> Children { get; set; }
    /// <summary>
    /// 构造函数
    /// </summary>
    public TableStructureTreeNode()
    {
        Children = new List<ColumnModel>();
    }
}