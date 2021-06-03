using System;

/******************************************************************************************************************
 * 
 * 
 * 说　明： 列信息(版本：Version1.0.0)
 * 作　者：YuXiaoWei
 * 日　期：2016/08/29
 * 修　改：
 * 参　考：http://www.oschina.net/code/snippet_584165_50417
 * 备　注：暂无...
 * 
 * 
 * ***************************************************************************************************************/
namespace Win.Models
{
    /// <summary>
    /// 列信息
    /// </summary>
    [Serializable]
    public class ColumnModel
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表说明/表描述
        /// </summary>
        public string TableDescription { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public string ColumnOrder { get; set; }

        /// <summary>
        /// 列名/字段名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 字段长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 精度/字节数
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// 小数位数
        /// </summary>
        public string Scale { get; set; }

        /// <summary>
        /// 是否自增标示列
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 可空
        /// </summary>
        public bool IsCanNull { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultVal { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}