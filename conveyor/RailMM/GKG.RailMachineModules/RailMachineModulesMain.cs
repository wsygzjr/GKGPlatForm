using GF_Gereric;
using Griffins.ImeIOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Plugin(Griffins.ImeIOT.MachineModulesMngAttribute.PLUGINKIND_Str, "{A86674EA-1764-4415-8185-F7DA9C4667FD}", "GKG.RailMachineModules")]

namespace GKG
{
    namespace MM
    {
        /// <summary>
        /// 基础运动控制机械手子机械模组
        /// </summary>
        [MachineModulesMngAttribute(RailMachineModulesConst.MMModelStr, RailMachineModulesConst.MMModelStr)]
        //[PluginSupportCompany("GKG")]
        class RailMachineModulesMain : GriffinsPluginMngClass, IMachineModulesPlugin
        {
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="pluginPath">插件路径</param>
            void IMachineModulesPlugin.Init(string pluginPath)
            {
            }

            //子机械模组名称
            string IMachineModulesMng.MMName { get => RailMachineModulesConst.MMName; }

            public IMachineModulesCabilityDef CretaeCabilityDef()
            {
                return new MachineModulesCabilityDef();
            }

            public IMMCmdExecutor CreateMMCmdExecutor(MMAlias alias, byte[] factoryCfgInfo)
            {
                return new RailMMCmdExecutor(alias, factoryCfgInfo);
            }
        }
    }
}
