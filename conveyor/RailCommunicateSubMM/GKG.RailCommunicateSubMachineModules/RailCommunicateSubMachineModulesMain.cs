using GF_Gereric;
using Griffins.ImeIOT;

[assembly: Plugin(Griffins.ImeIOT.SubMachineModulesMngAttribute.PLUGINKIND_Str, "{E5EF7883-A321-4B81-B777-8D2E70826739}", "GKG.RailCommunicateSubMachineModules")]

namespace GKG
{
    namespace SubMM
    {
        [SubMachineModulesMngAttribute(RailCommunicateSubMachineModulesConst.SubMMModelStr, RailCommunicateSubMachineModulesConst.SubMMModelStr)]
        class RailCommunicateSubMachineModulesMain : GriffinsPluginMngClass, ISubMachineModulesRunTimePlugin
        {
            void ISubMachineModulesRunTimePlugin.Init(string pluginPath)
            {
            }

            string ISubMachineModulesMng.SubMMName => RailCommunicateSubMachineModulesConst.SubMMName;

            public SubMMObjInfoList SubMMObjInfos => RailCommunicateSubMachineModulesConst.SubMMObjInfos;

            ISubMachineModulesCabilityDef ISubMachineModulesMng.CretaeCabilityDef()
            {
                return new SubMachineModulesCabilityDef();
            }

            ISubMMCmdExecutor ISubMachineModulesMng.CreateSubMMCmdExecutor(SubMMAlias alias, System.Guid subMMObjID, byte[] factoryCfgInfo)
            {
                switch (subMMObjID)
                {
                    case var id when id == RailCommunicateSubMachineModulesConst.SubMMObjInfos[0].SubMMObjID:
                        return new RailCommunicateSubMMCmdExecutor(alias, factoryCfgInfo);
                    default:
                        return null;
                }
            }
        }
    }
}
