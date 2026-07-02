using GF_Gereric;
using Griffins.ImeIOT;

[assembly: Plugin(Griffins.ImeIOT.SubMachineModulesMngAttribute.PLUGINKIND_Str, "{6EE04F1A-FD95-481A-92E4-B79A6378C5A7}", "Griffins.MotionCalculateSubMachineModules")]

namespace GKG.SubMM
{
    /// <summary>
    /// 基础运动控制机械手子机械模组
    /// </summary>
    [SubMachineModulesMngAttribute(MotionCalculateSubMachineModulesConst.SubMMModelStr)]
    internal
    //[PluginSupportCompany("GKG")]
    class MotionCalculateSubMachineModulesMain : GriffinsPluginMngClass, ISubMachineModulesRunTimePlugin
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pluginPath">插件路径</param>
        void ISubMachineModulesRunTimePlugin.Init(string pluginPath)
        {
        }

        //     子机械模组名称
        string ISubMachineModulesMng.SubMMName { get => MotionCalculateSubMachineModulesConst.SubMMName; }

        public SubMMObjInfoList SubMMObjInfos => MotionCalculateSubMachineModulesConst.SubMMObjInfos;

        /// <summary>
        /// 创建子机械模组（复合子机械模组）命令执行对象接口实例
        /// </summary>
        /// <param name="alias">子机械模组实例别名</param>
        /// <returns>子机械模组（复合子机械模组）命令执行对象接口实例</returns>

        ISubMachineModulesCabilityDef ISubMachineModulesMng.CretaeCabilityDef()
        {
            return new SubMachineModulesCabilityDef();
        }

        public ISubMachineModulesConfig CretaeSubMMConfig(SubMMAlias alias, Guid subMMObjID)
        {
            return null;
        }
        /// <summary>
        /// 创建子机械模组（复合子机械模组）命令执行对象接口实例
        /// </summary>
        /// <param name="alias">子机械模组实例别名</param>
        /// <param name="subMMObjID">实现对象ID</param>
        /// <param name="factoryCfgInfo">出厂配置参数</param>
        /// <returns>子机械模组（复合子机械模组）命令执行对象接口实例</returns>
        public ISubMMCmdExecutor CreateSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfgInfo)
        {
            return new MotionCalculateSubMMCmdExecutor(alias, subMMObjID, factoryCfgInfo);
        }
    }
}