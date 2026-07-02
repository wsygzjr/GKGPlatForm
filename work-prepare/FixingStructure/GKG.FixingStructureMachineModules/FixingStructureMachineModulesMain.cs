using GF_Gereric;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Plugin(Griffins.ImeIOT.MachineModulesMngAttribute.PLUGINKIND_Str, "{C2549475-3358-4805-98F8-15D342052644}", "GKG.FixingStructureMachineModules")]

namespace GKG
{
    namespace MM
    {
        /// <summary>
        /// 基础运动控制机械手子机械模组
        /// </summary>
        [MachineModulesMngAttribute(FixingStructureMachineModulesConst.MMModelStr, "WorkPrepare")]
        //[PluginSupportCompany("GKG")]
        class FixingStructureMachineModulesMain : GriffinsPluginMngClass, IMachineModulesPlugin
        {
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="pluginPath">插件路径</param>
            void IMachineModulesPlugin.Init(string pluginPath)
            {
            }

            //子机械模组名称
            string IMachineModulesMng.MMName { get => FixingStructureMachineModulesConst.MMName; }

            public IMachineModulesCabilityDef CretaeCabilityDef()
            {
                return new MachineModulesCabilityDef();
            }

            public IMMCmdExecutor CreateMMCmdExecutor(MMAlias alias, byte[] factoryCfgInfo)
            {
                return new FixingStructureMachineModulesCmdExecutor(alias, factoryCfgInfo);
            }
        }
    }
}
