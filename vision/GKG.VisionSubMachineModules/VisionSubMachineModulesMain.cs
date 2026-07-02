using GF_Gereric;
using Griffins.ImeIOT;

[assembly: Plugin(Griffins.ImeIOT.SubMachineModulesMngAttribute.PLUGINKIND_Str, "{709ACB79-7844-40F7-971A-1873BBDBBB4A}", "Griffins.VisionSubMachineModules")]

namespace GKG
{
    namespace SubMM
    {
        /// <summary>
        /// 基础运动控制机械手子机械模组
        /// </summary>
        [SubMachineModulesMngAttribute(VisionSubMachineModulesConst.SubMMModelStr, VisionSubMachineModulesConst.SubMMModelStr)]
        internal
        //[PluginSupportCompany("GKG")]
        class VisionSubMachineModulesMain : GriffinsPluginMngClass, ISubMachineModulesRunTimePlugin
        {
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="pluginPath">插件路径</param>
            void ISubMachineModulesRunTimePlugin.Init(string pluginPath)
            {
            }

            //     子机械模组名称
            string ISubMachineModulesMng.SubMMName { get => VisionSubMachineModulesConst.SubMMName; }

            public SubMMObjInfoList SubMMObjInfos => VisionSubMachineModulesConst.SubMMObjInfos;

            ISubMachineModulesCabilityDef ISubMachineModulesMng.CretaeCabilityDef()
            {
                return new SubMachineModulesCabilityDef();
            }

            /// <summary>
            /// 创建子机械模组（复合子机械模组）命令执行对象接口实例
            /// </summary>
            /// <param name="alias">子机械模组实例别名</param>
            /// <returns>子机械模组（复合子机械模组）命令执行对象接口实例</returns>
            public ISubMMCmdExecutor CreateSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfgInfo)
            {
                switch (subMMObjID.ToString())
                {
                    case "{6CB3CF76-8B68-4B1B-AD7B-E28C3D817244}":
                    default:
                        return new VisionSubMMCmdExecutor(alias, factoryCfgInfo);
                }
            }
        }
    }
}