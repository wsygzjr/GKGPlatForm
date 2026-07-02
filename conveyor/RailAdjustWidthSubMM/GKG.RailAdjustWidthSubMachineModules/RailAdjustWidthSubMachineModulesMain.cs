using GF_Gereric;
using Griffins.ImeIOT;

[assembly: Plugin(Griffins.ImeIOT.SubMachineModulesMngAttribute.PLUGINKIND_Str, "{709ACB79-7844-40F7-971A-1873BBDBBB4A}", "GKG.RailAdjustWidthSubMachineModules")]

namespace GKG
{
    namespace SubMM
    {
        [SubMachineModulesMngAttribute(RailAdjustWidthSubMachineModulesConst.SubMMModelStr, RailAdjustWidthSubMachineModulesConst.SubMMModelStr)]
        class RailAdjustWidthSubMachineModulesMain : GriffinsPluginMngClass, ISubMachineModulesRunTimePlugin
        {
            void ISubMachineModulesRunTimePlugin.Init(string pluginPath)
            {
            }

            string ISubMachineModulesMng.SubMMName => RailAdjustWidthSubMachineModulesConst.SubMMName;

            public SubMMObjInfoList SubMMObjInfos => RailAdjustWidthSubMachineModulesConst.SubMMObjInfos;

            ISubMachineModulesCabilityDef ISubMachineModulesMng.CretaeCabilityDef()
            {
                return new SubMachineModulesCabilityDef();
            }

            ISubMMCmdExecutor ISubMachineModulesMng.CreateSubMMCmdExecutor(SubMMAlias alias, System.Guid subMMObjID, byte[] factoryCfgInfo)
            {
                switch (subMMObjID)
                {
                    case var id when id == RailAdjustWidthSubMachineModulesConst.SubMMObjInfos[0].SubMMObjID:
                        return new RailAdjustWidthSubMMCmdExecutor(alias, factoryCfgInfo);
                    default:
                        return null;
                }
            }
        }
    }
}
