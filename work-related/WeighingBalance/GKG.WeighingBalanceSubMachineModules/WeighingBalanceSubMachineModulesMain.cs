using GF_Gereric;
using Griffins.ImeIOT;
using System;

[assembly: Plugin(Griffins.ImeIOT.SubMachineModulesMngAttribute.PLUGINKIND_Str, "{EE35A498-2D12-49EC-857B-853E56BB52F0}", "Griffins.WeighingBalanceSubMachineModules")]

namespace GKG.SubMM.Dispenser
{
    /// <summary>
    /// 基础运动控制机械手子机械模组
    /// </summary>
    [SubMachineModulesMngAttribute(WeighingBalanceSubMachineModulesConst.SubMMModelStr, "WorkClass")]

    //[PluginSupportCompany("GKG")]
    internal class WeighingBalanceSubMachineModulesMain : GriffinsPluginMngClass, ISubMachineModulesRunTimePlugin
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pluginPath">插件路径</param>
        void ISubMachineModulesRunTimePlugin.Init(string pluginPath)
        {
        }

        //     子机械模组名称
        string ISubMachineModulesMng.SubMMName => WeighingBalanceSubMachineModulesConst.SubMMName;

        SubMMObjInfoList ISubMachineModulesMng.SubMMObjInfos => WeighingBalanceSubMachineModulesConst.SubMMObjInfos;

        /// <summary>
        /// 创建子机械模组（复合子机械模组）命令执行对象接口实例
        /// </summary>
        /// <param name="alias">子机械模组实例别名</param>
        /// <returns>子机械模组（复合子机械模组）命令执行对象接口实例</returns>
        ISubMMCmdExecutor ISubMachineModulesMng.CreateSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfgInfo)
        {
            switch (subMMObjID)
            {
                case var id when id == WeighingBalanceSubMachineModulesConst.SubMMObjInfos[0].SubMMObjID:
                    return new WeighingBalanceSubMMCmdExecutor(alias, factoryCfgInfo);
                default:
                    throw new Exception($"不支持的子机械模组对象ID:{subMMObjID}");
            }
        }

        ISubMachineModulesCabilityDef ISubMachineModulesMng.CretaeCabilityDef()
        {
            return new SubMachineModulesCabilityDef();
        }


    }
}