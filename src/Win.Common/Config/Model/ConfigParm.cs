namespace Win.Common.Config.Model;
/// <summary>
/// 配置参数
/// </summary>
public class ConfigParm
{
    /// <summary>
    /// 默认数据库连接字符串
    /// </summary>
    public string DefaultConnString { get; set; }
    /// <summary>
    /// 默认数据库类型
    /// </summary>
    public string DefaultDbType { get; set; }
    /// <summary>
    /// 默认本地化
    /// </summary>
    public string DefaultCulture { get; set; }
    /// <summary>
    /// 全局引用
    /// </summary>
    public bool GlobalUsingEnabled { get; set; }
    /// <summary>
    /// 可空引用类型
    /// </summary>
    public bool NullableReferenceTypesEnabled { get; set; }
    /// <summary>
    /// 文件范围命名空间
    /// </summary>
    public bool FileScopedNamespaceEnabled { get; set; }
}