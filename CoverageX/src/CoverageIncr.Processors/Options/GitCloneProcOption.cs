namespace CoverageIncr.Processors.Options;

public class GitCloneProcOption
{
    /// <summary>
    /// 项目地址
    /// </summary>
    public required string Repository { get; set; }
    
    /// <summary>
    /// 开发分支
    /// 实际各个需求或功能模块的单独的开发分支
    /// </summary>
    public required string FeatureBranch { get; set; }
    
    /// <summary>
    /// 克隆开发分支到的目的地址
    /// </summary>
    public required string FeaturePath { get; set; }
    
    /// <summary>
    /// 部署分支
    /// 实际部署在服务器对外提供服务的部署分支
    /// </summary>
    public required string DeployBranch { get; set; }
    
    /// <summary>
    /// 克隆部署分支到的目的地址
    /// </summary>
    public required string DeployPath { get; set; }
    
    /// <summary>
    /// 用户名
    /// </summary>
    public required string UserName { get; set; }
    
    /// <summary>
    /// 密码
    /// </summary>
    public required string Password { get; set; }
}