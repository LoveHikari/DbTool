using System.Collections.Generic;
using Hikari.Common;
using Win.Models;

namespace Win.Common.Builder;
/// <summary>
/// 代码生成策略接口
/// </summary>
public abstract class BuilderStragety
{
    #region 字段
    protected List<ColumnModel> _fieldList;  //选择的字段集合
    protected string _modelPath;  //实体类的命名空间
    protected string _modelName;  //实体类名
    protected string _repositoryPath;  //仓储类的命名空间
    protected string _repositoryName;  //仓储类名
    protected string _applicationPath;  //业务类的命名空间
    protected string _applicationName;  //业务类名
    #endregion

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="fieldList">选择的字段集合</param>
    /// <param name="modelPath">实体类的命名空间</param>
    /// <param name="modelPrefix">实体类的前缀</param>
    /// <param name="modelSuffix">实体类的后缀</param>
    /// <param name="repositoryPath">仓储类的命名空间</param>
    /// <param name="repositoryPrefix">仓储类的前缀</param>
    /// <param name="repositorySuffix">仓储类的后缀</param>
    /// <param name="applicationPath">业务类的命名空间</param>
    /// <param name="applicationPrefix">业务类的前缀</param>
    /// <param name="applicationSuffix">业务类的后缀</param>
    protected BuilderStragety(List<ColumnModel> fieldList, string modelPath, string modelPrefix, string modelSuffix,
        string repositoryPath, string repositoryPrefix, string repositorySuffix,
        string applicationPath, string applicationPrefix, string applicationSuffix)
    {

        _fieldList = fieldList;
        _modelPath = modelPath;
        string tableName = _fieldList[0].TableName;
        _modelName = modelPrefix + tableName.ToPascalCase() + modelSuffix;

        _repositoryPath = repositoryPath;
        _repositoryName = repositoryPrefix + tableName.ToPascalCase() + repositorySuffix;

        _applicationPath = applicationPath;
        _applicationName = applicationPrefix + tableName.ToPascalCase() + applicationSuffix;
    }


    /// <summary>
    /// 生成完整Model类
    /// </summary>
    internal abstract string CreatModel();
    /// <summary>
    /// 生成仓储接口
    /// </summary>
    /// <returns></returns>
    internal abstract string CreatRepositoryInterface();

    /// <summary>
    /// 生成仓储类
    /// </summary>
    /// <returns></returns>
    internal abstract string CreatRepository();

    /// <summary>
    /// 生成业务接口
    /// </summary>
    /// <returns></returns>
    internal abstract string CreatApplicationInterface();

    /// <summary>
    /// 生成业务类
    /// </summary>
    /// <returns></returns>
    internal abstract string CreatApplication();
}