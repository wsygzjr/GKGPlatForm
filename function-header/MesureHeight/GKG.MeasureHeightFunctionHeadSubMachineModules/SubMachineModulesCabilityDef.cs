using Griffins;
using Griffins.ImeIOT;

namespace GKG
{
    namespace SubMM
    {
        public class SubMachineModulesCabilityDef : ISubMachineModulesCabilityDef
        {
            ImeCompEventDefInfoList ISubMachineModulesCabilityDef.Events => MeasureHeightFunctionHeadSubMachineModulesConst.Events;

            ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.Methods => MeasureHeightFunctionHeadSubMachineModulesConst.Methods;

            ImeCompPropDefInfoList ISubMachineModulesCabilityDef.UIDataObjProps => null!;

            ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.UICommands => null!;

            DevicePropertyInfoList ISubMachineModulesCabilityDef.DeviceProps => null!;

            ISubMachineModulesDefSvr ISubMachineModulesCabilityDef.CreateISubMachineModulesDefSvr(SubMMAlias alias, byte[] factoryCfgInfo, GFBaseTypePropValueList devicePropValues)
            {
                return null!;
            }
        }
    }
}