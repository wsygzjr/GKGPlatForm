using GF_Gereric;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Plugin(Griffins.ImeIOT.SubMachineModulesMngAttribute.PLUGINKIND_Str, "{709ACB79-7844-40F7-971A-1873BBDBBB4A}", "GKG.RailMotorSubMachineModules")]

namespace GKG
{
    namespace SubMM
    {
        /// <summary>
        /// 基础运动控制机械手子机械模组
        /// </summary>
        [SubMachineModulesMngAttribute(RailWorkStationSubMachineModulesConst.SubMMModelStr, RailWorkStationSubMachineModulesConst.SubMMModelStr)]
        //[PluginSupportCompany("GKG")]
        class RailWorkStationSubMachineModulesMain : GriffinsPluginMngClass, ISubMachineModulesRunTimePlugin
        {
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="pluginPath">插件路径</param>
            void ISubMachineModulesRunTimePlugin.Init(string pluginPath)
            {

            }

            //     子机械模组名称
            string ISubMachineModulesMng.SubMMName { get => RailWorkStationSubMachineModulesConst.SubMMName; }

            public SubMMObjInfoList SubMMObjInfos => RailWorkStationSubMachineModulesConst.SubMMObjInfos;

           

            ISubMachineModulesCabilityDef ISubMachineModulesMng.CretaeCabilityDef()
            {
                return new SubMachineModulesCabilityDef();
            }

            ISubMMCmdExecutor ISubMachineModulesMng.CreateSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfgInfo)
            {
                switch (subMMObjID)
                {
                    case var id when id == RailWorkStationSubMachineModulesConst.SubMMObjInfos[0].SubMMObjID:
                        return new RailWorkStationSubMMCmdExecutor(alias, factoryCfgInfo);
                    default:
                        return null;
                }
            }
        }
    }
}
