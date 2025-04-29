namespace Win.Common.Builder;
/// <summary>
/// 环境角色
/// </summary>
public class BuilderContext
{
    private BuilderStragety _builderStragety;

    public BuilderContext(BuilderStragety builderStragety)
    {
        _builderStragety = builderStragety;
    }

    /// <summary>
    /// 生成完整Model类
    /// </summary>
    public string CreatModel()
    {
        return _builderStragety.CreatModel();
    }

    /// <summary>
    /// 生成仓储接口
    /// </summary>
    /// <returns></returns>
    public string CreatRepositoryInterface()
    {
        return _builderStragety.CreatRepositoryInterface();
    }

    /// <summary>
    /// 生成仓储类
    /// </summary>
    /// <returns></returns>
    public string CreatRepository()
    {
        return _builderStragety.CreatRepository();
    }

    /// <summary>
    /// 生成业务接口
    /// </summary>
    /// <returns></returns>
    public string CreatApplicationInterface()
    {
        return _builderStragety.CreatApplicationInterface();
    }

    /// <summary>
    /// 生成业务类
    /// </summary>
    /// <returns></returns>
    public string CreatApplication()
    {
        return _builderStragety.CreatApplication();
    }
}