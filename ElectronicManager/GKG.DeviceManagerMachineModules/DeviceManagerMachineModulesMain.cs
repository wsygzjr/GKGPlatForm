using GF_Gereric;
using Griffins.ImeIOT;

[assembly: Plugin(Griffins.ImeIOT.MachineModulesMngAttribute.PLUGINKIND_Str, "{6F512E36-2F4A-4E32-B4B6-D5D3317D1A4D}", "GKG.LoadUnloadMachineModules")]

namespace GKG
{
    namespace MM
    {
        /// <summary>总机械模组插件入口：向宿主注册上下料总模块及其执行器工厂。</summary>
        [MachineModulesMngAttribute(DeviceManagerMachineModulesConst.MMModelStr, "DeviceManage")]
        class DeviceManagerMachineModulesMain : GriffinsPluginMngClass, IMachineModulesPlugin
        {
            /// <summary>插件初始化入口；当前无额外启动逻辑。</summary>
            void IMachineModulesPlugin.Init(string pluginPath)
            {
            }

            string IMachineModulesMng.MMName => DeviceManagerMachineModulesConst.MMName;

            public IMachineModulesCabilityDef CretaeCabilityDef()
            {
                return new MachineModulesCabilityDef();
            }

            public IMMCmdExecutor CreateMMCmdExecutor(MMAlias alias, byte[] factoryCfgInfo)
            {
                return new DeviceManagerMMCmdExecutor(alias, factoryCfgInfo);
            }
        }
    }
}
