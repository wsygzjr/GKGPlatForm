using GF_Gereric;
using Griffins.ImeIOT;
using DispenserMachineModules;

[assembly: Plugin(MachineModulesMngAttribute.PLUGINKIND_Str, "{B74878CA-ECB6-4CDA-98B9-E043DFDE55AB}", "Griffins.DispenserMachineModules")]

namespace GKG
{
    namespace MM
    {
        /// <summary>
        /// 印刷机械模组
        /// </summary>
        [BscRunMng(DispenserMachineModulesConst.MMNumberStr)]
        public class DispenserMachineModulesMain : GriffinsPluginMngClass, IMachineModulesPlugin
        {
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="pluginPath">插件路径</param>
            void IMachineModulesPlugin.Init(string pluginPath)
            {
            }

            /// <summary>
            /// 机械模组名称
            /// </summary>
            string IMachineModulesMng.MMName { get { return DispenserMachineModulesConst.MMName; } }

            /// <summary>
            /// 创建机械模组命令执行对象接口实例
            /// </summary>
            /// <param name="alias">机械模组实例别名</param>
            /// <returns>机械模组命令执行对象接口实例</returns>
            IMMCmdExecutor IMachineModulesMng.CreateMMCmdExecutor(MMAlias alias)
            {
                return new MMCmdExecutor(alias);
            }

            IMachineModulesCabilityDef IMachineModulesMng.CretaeCabilityDef()
            {
                return new MachineModulesCabilityDef();
            }

            IMachineModulesDesignTime IMachineModulesMng.CretaeDesignTime(MMAlias alias)
            {
                throw new NotImplementedException();
            }
        }
    }
}