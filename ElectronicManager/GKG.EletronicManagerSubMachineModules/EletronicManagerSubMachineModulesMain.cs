using GF_Gereric;
using Griffins.ImeIOT;
using System;

[assembly: Plugin(
    Griffins.ImeIOT.SubMachineModulesMngAttribute.PLUGINKIND_Str, 
    "{BC48C9D7-0041-47B5-8CE8-9EA346716484}", 
    "GKG.EletronicManagerSubMachineModules"
)]

namespace GKG.SubMM;

/// <summary>
/// 电子管理子机械模组主插件类
/// 实现 Griffins 插件框架的子机械模组运行时插件接口
/// </summary>
[SubMachineModulesMngAttribute(EletronicManagerSubMachineModulesConst.SubMMModelStr, "DeviceManage")]
public class EletronicManagerSubMachineModulesMain : GriffinsPluginMngClass, ISubMachineModulesRunTimePlugin
{
    #region 插件初始化

    /// <summary>
    /// 插件初始化
    /// </summary>
    /// <param name="pluginPath">插件路径</param>
    void ISubMachineModulesRunTimePlugin.Init(string pluginPath)
    {
        // 插件初始化逻辑（如需要可在此加载配置文件等）
    }
    Guid SubMMObjID = Guid.Parse("D019035E-FCBF-4408-BDE4-7A024B358AAC");
    #endregion

    #region 子机械模组信息

    /// <summary>
    /// 子机械模组名称
    /// </summary>
    string ISubMachineModulesMng.SubMMName => EletronicManagerSubMachineModulesConst.SubMMName;

    SubMMObjInfoList ISubMachineModulesMng.SubMMObjInfos => new SubMMObjInfoList
    {
        new SubMMObjInfo()
        {
            SubMMObjID = SubMMObjID,
            SubMMObjName="默认"
        }
    };


    #endregion

    #region 工厂方法

    /// <summary>
    /// 创建能力定义
    /// </summary>
    /// <returns>能力定义实例</returns>
    public ISubMachineModulesCabilityDef CretaeCabilityDef()
    {
        return new SubMachineModulesCabilityDef();
    }
    public ISubMMCmdExecutor CreateSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfgInfo)
    {
        return new EletronicManageSubMMCmdExecutor(alias,subMMObjID,factoryCfgInfo);
    }



    #endregion
}
