using Griffins;
using Griffins.ImeIOT;

namespace GKG.SubMM.Dispenser
{
    public class SubMachineModulesCabilityDef : ISubMachineModulesCabilityDef
    {
        ImeCompEventDefInfoList ISubMachineModulesCabilityDef.Events => DispensingFunctionHeadSubMachineModulesConst.Events;

        ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.Methods => DispensingFunctionHeadSubMachineModulesConst.Methods;

        ImeCompPropDefInfoList ISubMachineModulesCabilityDef.UIDataObjProps => null!;

        ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.UICommands => null!;

        DevicePropertyInfoList ISubMachineModulesCabilityDef.DeviceProps => null!;

        ISubMachineModulesDefSvr ISubMachineModulesCabilityDef.CreateISubMachineModulesDefSvr(SubMMAlias alias, byte[] factoryCfgInfo, GFBaseTypePropValueList devicePropValues)
        {
            return null!;
        }
    }
}