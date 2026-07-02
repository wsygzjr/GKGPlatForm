using GF_Gereric;
using Griffins.ImeIOT;

[assembly: Plugin(Griffins.ImeIOT.SubMachineModulesMngAttribute.PLUGINKIND_Str, "{709ACB79-7844-40F7-971A-1873BBDBBB4A}", "GKG.RailMotorSubMachineModules")]

namespace GKG
{
    namespace SubMM
    {
        /// <summary>
        /// 轨道运输电机子机械模组
        /// </summary>
        [SubMachineModulesMngAttribute(RailMotorSubMachineModulesConst.SubMMModelStr, RailMotorSubMachineModulesConst.SubMMModelStr)]
        class RailMotorSubMachineModulesMain : GriffinsPluginMngClass, ISubMachineModulesRunTimePlugin
        {
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="pluginPath">插件路径</param>
            void ISubMachineModulesRunTimePlugin.Init(string pluginPath)
            {
            }

            string ISubMachineModulesMng.SubMMName => RailMotorSubMachineModulesConst.SubMMName;

            public SubMMObjInfoList SubMMObjInfos => RailMotorSubMachineModulesConst.SubMMObjInfos;

            ISubMachineModulesCabilityDef ISubMachineModulesMng.CretaeCabilityDef()
            {
                return new SubMachineModulesCabilityDef();
            }

            ISubMMCmdExecutor ISubMachineModulesMng.CreateSubMMCmdExecutor(SubMMAlias alias, System.Guid subMMObjID, byte[] factoryCfgInfo)
            {
                switch (subMMObjID)
                {
                    case var id when id == RailMotorSubMachineModulesConst.SubMMObjInfos[0].SubMMObjID:
                        return new RailMotorSubMMCmdExecutor(alias, factoryCfgInfo);
                    default:
                        return null;
                }
            }
        }
    }
}
