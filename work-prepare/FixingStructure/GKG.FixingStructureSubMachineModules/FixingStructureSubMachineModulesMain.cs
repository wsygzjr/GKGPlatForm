using GF_Gereric;
using Griffins.ImeIOT;
using System;

[assembly: Plugin(Griffins.ImeIOT.SubMachineModulesMngAttribute.PLUGINKIND_Str, "{0C8C3F0E-9B89-48AE-806D-DAB45C8DE352}", "GKG.FixingStructureSubMachineModules")]

namespace GKG.SubMM
{
    [SubMachineModulesMngAttribute(FixingStructureSubMachineModulesConst.SubMMModelStr, "WorkPrepare")]
    internal class FixingStructureSubMachineModulesMain : GriffinsPluginMngClass, ISubMachineModulesRunTimePlugin
    {
        void ISubMachineModulesRunTimePlugin.Init(string pluginPath)
        {
        }

        string ISubMachineModulesMng.SubMMName => FixingStructureSubMachineModulesConst.SubMMName;

        public SubMMObjInfoList SubMMObjInfos => FixingStructureSubMachineModulesConst.SubMMObjInfos;

        ISubMachineModulesCabilityDef ISubMachineModulesMng.CretaeCabilityDef()
        {
            return new SubMachineModulesCabilityDef();
        }

        ISubMMCmdExecutor ISubMachineModulesMng.CreateSubMMCmdExecutor(SubMMAlias alias, Guid subMMObjID, byte[] factoryCfgInfo)
        {
            switch (subMMObjID)
            {
                case var id when id == FixingStructureSubMachineModulesConst.SubMMObjInfos[0].SubMMObjID:
                    return new CylinderFixSubMMCmdExecutor(alias, factoryCfgInfo);
                case var id when id == FixingStructureSubMachineModulesConst.SubMMObjInfos[1].SubMMObjID:
                    return new AxisFixSubMMCmdExecutor(alias, factoryCfgInfo);
                default:
                    return null!;
            }
        }
    }
}
