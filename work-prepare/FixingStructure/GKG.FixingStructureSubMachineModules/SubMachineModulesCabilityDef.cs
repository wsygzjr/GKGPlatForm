using GF_Gereric;
using Griffins;
using Griffins.ImeIOT;

namespace GKG.SubMM
{
    internal class SubMachineModulesCabilityDef : ISubMachineModulesCabilityDef
    {
        ImeCompEventDefInfoList ISubMachineModulesCabilityDef.Events => FixingStructureSubMachineModulesConst.CompEvents;

        ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.Methods => FixingStructureSubMachineModulesConst.CompMethods;

        ImeCompPropDefInfoList ISubMachineModulesCabilityDef.UIDataObjProps => null!;

        ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.UICommands => null!;

        DevicePropertyInfoList ISubMachineModulesCabilityDef.DeviceProps => null!;

        ISubMachineModulesDefSvr ISubMachineModulesCabilityDef.CreateISubMachineModulesDefSvr(SubMMAlias alias, byte[] factoryCfgInfo, GFBaseTypePropValueList devicePropValues)
        {
            return null!;
        }
    }
}
