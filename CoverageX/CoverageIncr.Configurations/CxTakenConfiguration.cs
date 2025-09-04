using CoverageIncr.Shared.Pipelines;

namespace CoverageIncr.Configurations;

public class CxTakenConfiguration
{
    // receivers 是一个动态的 map，key=receiver名称，value=原始配置（方便后续绑定到具体Options类）
    public Dictionary<string, Dictionary<string, object>> Receivers { get; set; } = new();
    
    public Dictionary<string, Dictionary<string, object>> Processors { get; set; } = new();
    
    public Dictionary<string, Dictionary<string, object>> Exporters { get; set; } = new();

    // pipelines 同理可以是一个map结构
    public Dictionary<string, PipelineScope> Pipelines { get; set; } = new();
    
    // /// <summary>
    // /// 报告类型
    // /// </summary>
    // public ReportType ReportType { get; set; }
    //
    // /// <summary>
    // /// 基准分支
    // /// 用于与FeatureBranch进行比较计算增量的基准分支
    // /// </summary>
    // public string BaseBranch { get; set; }
    //
    // /// <summary>
    // /// 开发分支
    // /// 实际各个需求或功能模块的单独的开发分支
    // /// </summary>
    // public string FeatureBranch { get; set; }
    //
    // /// <summary>
    // /// 部署分支
    // /// 实际部署在服务器对外提供服务的部署分支
    // /// </summary>
    // public string DeployBranch { get; set; }
    //
    // /// <summary>
    // /// 覆盖率环境
    // /// </summary>
    // public string Env { get; set; }
    //
    // /// <summary>
    // /// 项目标识
    // /// </summary>
    // public string ProjectUk { get; set; }
    //
    // /// <summary>
    // /// 覆盖率存放目录
    // /// </summary>
    // public string CoverageDir { get; set; }
    //
    // /// <summary>
    // /// 报告存放目录
    // /// </summary>
    // public string ReportDir { get; set; }
}