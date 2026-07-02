using GF_Gereric;
using Griffins.ImeIOT;
using System;

[assembly: Plugin(Griffins.ImeIOT.SubMachineModulesMngAttribute.PLUGINKIND_Str, "{ED7411D1-FE76-4680-BDC2-96981F58D523}", "GKG.MeasureHeightFunctionHeadSubMachineModules")]

namespace GKG
{
    namespace SubMM
    {
        /// <summary>
        /// 基础运动控制机械手子机械模组
        /// </summary>
        [SubMachineModulesMngAttribute(MeasureHeightFunctionHeadSubMachineModulesConst.SubMMModelStr, "FunctionHeader")]
        class MeasureHeightFunctionHeadSubMachineModulesMain : GriffinsPluginMngClass, ISubMachineModulesRunTimePlugin
        {
            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="pluginPath">插件路径</param>
            void ISubMachineModulesRunTimePlugin.Init(string pluginPath)
            {
            }

            //     子机械模组名称
            string ISubMachineModulesMng.SubMMName { get => MeasureHeightFunctionHeadSubMachineModulesConst.SubMMName; }

            SubMMObjInfoList ISubMachineModulesMng.SubMMObjInfos => MeasureHeightFunctionHeadSubMachineModulesConst.SubMMObjInfos;

            /// <summary>
            /// 创建子机械模组（复合子机械模组）命令执行对象接口实例
            /// </summary>
            /// <param name="alias">子机械模组实例别名</param>
            /// <returns>子机械模组（复合子机械模组）命令执行对象接口实例</returns>
            ISubMMCmdExecutor ISubMachineModulesMng.CreateSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfgInfo)
            {
                return new MeasureHeightFunctionHeadSubMMCmdExecutor(alias, factoryCfgInfo);
            }

            ISubMachineModulesCabilityDef ISubMachineModulesMng.CretaeCabilityDef()
            {
                return new SubMachineModulesCabilityDef();
            }
        }
    }
}