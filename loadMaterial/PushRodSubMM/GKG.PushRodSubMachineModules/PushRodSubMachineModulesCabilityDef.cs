using Griffins;
using Griffins.ImeIOT;

namespace GKG
{
    namespace SubMM
    {
        class PushRodSubMachineModulesCabilityDef : ISubMachineModulesCabilityDef
        {
            ImeCompEventDefInfoList ISubMachineModulesCabilityDef.Events => PushRodSubMachineModulesConst.CompEvents;

            ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.Methods => PushRodSubMachineModulesConst.CompMethods;

            ImeCompPropDefInfoList ISubMachineModulesCabilityDef.UIDataObjProps => null;

            ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.UICommands => null;

            DevicePropertyInfoList ISubMachineModulesCabilityDef.DeviceProps => throw new System.NotImplementedException();

            ISubMachineModulesDefSvr ISubMachineModulesCabilityDef.CreateISubMachineModulesDefSvr(SubMMAlias alias, byte[] factoryCfgInfo, GFBaseTypePropValueList devicePropValues)
            {
                return null!;
            }
        }
    }
}
