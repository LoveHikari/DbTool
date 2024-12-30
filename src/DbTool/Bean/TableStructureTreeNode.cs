using Win.Models;

namespace DbTool.Bean;
/// <summary>
/// 数据库表结构树节点
/// </summary>
public class TableStructureTreeNode : ColumnModel
{
    public List<TableStructureTreeNode> Children { get; set; }

    public TableStructureTreeNode()
    {
        Children = new List<TableStructureTreeNode>();
    }
}