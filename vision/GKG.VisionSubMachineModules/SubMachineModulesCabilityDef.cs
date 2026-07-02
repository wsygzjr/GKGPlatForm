using Griffins;
using Griffins.ImeIOT;

namespace GKG
{
	namespace SubMM
	{
		class SubMachineModulesCabilityDef : ISubMachineModulesCabilityDef
		{
            ImeCompEventDefInfoList ISubMachineModulesCabilityDef.Events => VisionSubMachineModulesConst.Events;

            ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.Methods => VisionSubMachineModulesConst.Methods;

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