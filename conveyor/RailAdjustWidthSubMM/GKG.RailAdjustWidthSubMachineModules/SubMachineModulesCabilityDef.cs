using GF_Gereric;
using Griffins;
using Griffins.ImeIOT;

namespace GKG
{
	namespace SubMM
	{
		class SubMachineModulesCabilityDef : ISubMachineModulesCabilityDef
		{
            ImeCompEventDefInfoList ISubMachineModulesCabilityDef.Events => RailAdjustWidthSubMachineModulesConst.CompEvents;

            ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.Methods => RailAdjustWidthSubMachineModulesConst.CompMethods;

            ImeCompPropDefInfoList ISubMachineModulesCabilityDef.UIDataObjProps => null;

            ImeCompMethodDefInfoList ISubMachineModulesCabilityDef.UICommands => null;


            DevicePropertyInfoList ISubMachineModulesCabilityDef.DeviceProps => null;

            ISubMachineModulesDefSvr ISubMachineModulesCabilityDef.CreateISubMachineModulesDefSvr(SubMMAlias alias, byte[] factoryCfgInfo, GFBaseTypePropValueList devicePropValues)
            {
                return null!;
            }
        }
	}
}
